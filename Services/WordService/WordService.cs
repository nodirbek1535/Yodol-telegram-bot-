using System.Text.Json;
using Yodol_telegram_bot_.Models.Word;

namespace Yodol_telegram_bot_.Services.WordService;

public sealed class WordService : IWordService
{
    private static readonly Lock FileLock = new();
    private readonly string _path;

    public WordService()
    {
        var storageDirectory = Path.Combine(AppContext.BaseDirectory, "Storage");
        Directory.CreateDirectory(storageDirectory);
        _path = Path.Combine(storageDirectory, "words.json");
    }

    public List<Word> GetAllWords()
    {
        lock (FileLock)
        {
            if (!File.Exists(_path))
            {
                return [];
            }

            var json = File.ReadAllText(_path);
            return JsonSerializer.Deserialize<List<Word>>(json) ?? [];
        }
    }

    public List<Word> GetChatWords(long chatId)
    {
        return GetAllWords().Where(w => w.ChatId == chatId).ToList();
    }

    public (Word word, bool isNew) AddWord(long chatId, string english, string uzbek, DateTime? deadline = null)
    {
        var normalizedEnglish = english.Trim();
        var normalizedUzbek = uzbek.Trim();

        if (string.IsNullOrWhiteSpace(normalizedEnglish) || string.IsNullOrWhiteSpace(normalizedUzbek))
        {
            throw new ArgumentException("English and Uzbek values are required.");
        }

        var words = GetAllWords();

        var existing = words.FirstOrDefault(w =>
            w.ChatId == chatId &&
            string.Equals(w.English, normalizedEnglish, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(w.Uzbek, normalizedUzbek, StringComparison.OrdinalIgnoreCase));

        if (existing is not null)
        {
            existing.CreatedDate = DateTime.Now;
            existing.IsLearned = false;
            existing.Deadline = deadline;
            existing.LastAskedTime = null;
            existing.RepeatCount = 0;

            SaveWords(words);
            return (existing, false);
        }

        var word = new Word
        {
            ChatId = chatId,
            English = normalizedEnglish,
            Uzbek = normalizedUzbek,
            CreatedDate = DateTime.Now,
            Deadline = deadline,
            IsLearned = false,
            RepeatCount = 0
        };

        words.Add(word);
        SaveWords(words);

        return (word, true);
    }

    public int GetTodayUnlearnedCount(long chatId)
    {
        var today = DateTime.Now.Date;

        return GetAllWords().Count(w =>
            w.ChatId == chatId &&
            w.CreatedDate.Date == today &&
            !w.IsLearned &&
            (w.Deadline is null || w.Deadline >= DateTime.Now));
    }

    private void SaveWords(List<Word> words)
    {
        lock (FileLock)
        {
            var json = JsonSerializer.Serialize(words, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_path, json);
        }
    }
}

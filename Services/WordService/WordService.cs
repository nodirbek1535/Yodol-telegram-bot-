//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using System.Text.Json;
using Yodol_telegram_bot_.Models.Word;

namespace Yodol_telegram_bot_.Services.WordService
{
    public class WordService
    {
        private readonly string _path = "Storage/words.json";

        public List<Word> GetAllWords()
        {
            if (!File.Exists(_path))
                return new List<Word>();

            var json = File.ReadAllText(_path);

            return JsonSerializer.Deserialize<List<Word>>(json) ?? new List<Word>();
        }

        private void SaveWords(List<Word> words)
        {
            var json = JsonSerializer.Serialize(words, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(_path, json);
        }

        public (Word word, bool isNew) AddWord(
            long chatId,
            string english,
            string uzbek,
            DateTime? deadline = null)
        {
            var words = GetAllWords();

            var existing = words.FirstOrDefault(w =>
                w.ChatId == chatId &&
                w.English.ToLower() == english.ToLower() &&
                w.Uzbek.ToLower() == uzbek.ToLower());

            if (existing != null)
            {
                existing.CreatedDate = DateTime.Now;
                existing.IsLearned = false;
                existing.Deadline = deadline;
                existing.LastAskedTime = null;
                existing.RepeatCount = 0;

                SaveWords(words);
                return (existing, false); // 🔁 update
            }

            var word = new Word
            {
                ChatId = chatId,
                English = english,
                Uzbek = uzbek,
                CreatedDate = DateTime.Now,
                Deadline = deadline,
                IsLearned = false,
                RepeatCount = 0
            };

            words.Add(word);
            SaveWords(words);

            return (word, true); // ✅ new
        }

        public List<Word> GetTodayWords()
        {
            var today = DateTime.Now.Date;

            return GetAllWords()
                .Where(w => w.CreatedDate.Date == today)
                .ToList();
        }

        public List<Word> GetWordsForReminder()
        {
            var now = DateTime.Now;

            return GetAllWords()
                .Where(w =>
                w.IsLearned == false &&
                w.Deadline != null &&
                now <= w.Deadline &&
                (w.LastAskedTime == null || (now - w.LastAskedTime.Value).TotalMinutes >= 120)
                )
                .ToList();
        }

        public void MarkIsLearned(Guid id)
        {
            var words = GetAllWords();

            var word = words.FirstOrDefault(w => w.WordId == id);

            if (word != null)
            {
                word.IsLearned = true;
                SaveWords(words);
            }
        }

        public void UpdateAsked(Guid id)
        {
            var words = GetAllWords();

            var word = words.FirstOrDefault(w => w.WordId == id);

            if (word != null)
            {
                word.LastAskedTime = DateTime.Now;
                word.RepeatCount++;
                SaveWords(words);
            }
        }

        public int GetTodayUnlearnedCount(long chatId)
        {
            var today = DateTime.Now.Date;

            return GetAllWords()
                .Count(w =>
                    w.ChatId == chatId &&
                    w.CreatedDate.Date == today &&
                    !w.IsLearned &&
                    (w.Deadline == null || w.Deadline >= DateTime.Now)
                );
        }
    }
}

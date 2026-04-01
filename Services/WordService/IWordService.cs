using Yodol_telegram_bot_.Models.Word;

namespace Yodol_telegram_bot_.Services.WordService;

public interface IWordService
{
    List<Word> GetAllWords();
    List<Word> GetChatWords(long chatId);
    (Word word, bool isNew) AddWord(long chatId, string english, string uzbek, DateTime? deadline = null);
    int GetTodayUnlearnedCount(long chatId);
}

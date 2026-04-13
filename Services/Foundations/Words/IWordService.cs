//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Yodol_telegram_bot_.Models.Words;

namespace Yodol_telegram_bot_.Services.Foundations.Words
{
    public interface IWordService
    {
        ValueTask<Word> AddWordAsync(Word word);
        ValueTask<List<Word>> RetrieveWordsByPackageIdAsync(Guid packageId);
        ValueTask<List<Word>> RetrieveWordsByUserTelegramIdAsync(long userTelegramId);
        ValueTask<Word?> RetrieveWordByIdAsync(Guid wordId);
        ValueTask<Word> ModifyWordAsync(Word word);
        ValueTask<Word> RemoveWordAsync(Word word);
    }
}

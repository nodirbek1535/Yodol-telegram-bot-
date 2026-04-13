//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Yodol_telegram_bot_.Models.Words;

namespace Yodol_telegram_bot_.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Word> InsertWordAsync(Word word);
        ValueTask<List<Word>> SelectAllWordsAsync();
        ValueTask<Word?> SelectWordByIdAsync(Guid wordId);
        ValueTask<List<Word>> SelectWordsByPackageIdAsync(Guid packageId);
        ValueTask<List<Word>> SelectWordsByUserTelegramIdAsync(long userTelegramId);
        ValueTask<Word> UpdateWordAsync(Word word);
        ValueTask<Word> DeleteWordAsync(Word word);
    }
}

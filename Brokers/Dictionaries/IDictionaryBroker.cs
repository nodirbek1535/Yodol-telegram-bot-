using Yodol_telegram_bot_.Models.Dictionaries;

namespace Yodol_telegram_bot_.Brokers.Dictionaries
{
    public interface IDictionaryBroker
    {
        ValueTask<List<DictionaryEntry>> GetWordDetailsAsync(string word);
    }
}

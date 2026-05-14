using Yodol_telegram_bot_.Models.Dictionaries;

namespace Yodol_telegram_bot_.Services.Foundations.Dictionaries
{
    public interface IDictionaryService
    {
        ValueTask<DictionaryEntry> RetrieveWordDetailsAsync(string word);
    }
}

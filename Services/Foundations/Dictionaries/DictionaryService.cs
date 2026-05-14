using Yodol_telegram_bot_.Brokers.Dictionaries;
using Yodol_telegram_bot_.Brokers.Loggings;
using Yodol_telegram_bot_.Models.Dictionaries;

namespace Yodol_telegram_bot_.Services.Foundations.Dictionaries
{
    public class DictionaryService : IDictionaryService
    {
        private readonly IDictionaryBroker dictionaryBroker;
        private readonly ILoggingBroker loggingBroker;

        public DictionaryService(
            IDictionaryBroker dictionaryBroker,
            ILoggingBroker loggingBroker)
        {
            this.dictionaryBroker = dictionaryBroker;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<DictionaryEntry> RetrieveWordDetailsAsync(string word)
        {
            try
            {
                List<DictionaryEntry> entries = await this.dictionaryBroker.GetWordDetailsAsync(word);
                
                return entries?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this.loggingBroker.LogError(ex);
                return null;
            }
        }
    }
}

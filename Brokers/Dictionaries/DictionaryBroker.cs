using System.Text.Json;
using Yodol_telegram_bot_.Models.Dictionaries;

namespace Yodol_telegram_bot_.Brokers.Dictionaries
{
    public class DictionaryBroker : IDictionaryBroker
    {
        private readonly HttpClient httpClient;

        public DictionaryBroker(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = new Uri("https://api.dictionaryapi.dev/api/v2/");
        }

        public async ValueTask<List<DictionaryEntry>> GetWordDetailsAsync(string word)
        {
            try
            {
                var response = await this.httpClient.GetAsync($"entries/en/{word}");
                
                if (!response.IsSuccessStatusCode)
                {
                    return new List<DictionaryEntry>();
                }

                string content = await response.Content.ReadAsStringAsync();
                
                return JsonSerializer.Deserialize<List<DictionaryEntry>>(content);
            }
            catch
            {
                return new List<DictionaryEntry>();
            }
        }
    }
}

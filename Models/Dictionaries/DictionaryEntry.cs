using System.Text.Json.Serialization;

namespace Yodol_telegram_bot_.Models.Dictionaries
{
    public class DictionaryEntry
    {
        [JsonPropertyName("word")]
        public string Word { get; set; }

        [JsonPropertyName("phonetics")]
        public List<Phonetic> Phonetics { get; set; }

        [JsonPropertyName("meanings")]
        public List<Meaning> Meanings { get; set; }
    }
}

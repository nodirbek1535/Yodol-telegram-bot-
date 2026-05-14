using System.Text.Json.Serialization;

namespace Yodol_telegram_bot_.Models.Dictionaries
{
    public class Phonetic
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("audio")]
        public string Audio { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace Yodol_telegram_bot_.Models.Dictionaries
{
    public class Definition
    {
        [JsonPropertyName("definition")]
        public string Text { get; set; }

        [JsonPropertyName("example")]
        public string Example { get; set; }
    }
}

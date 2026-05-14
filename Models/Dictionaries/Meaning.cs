using System.Text.Json.Serialization;

namespace Yodol_telegram_bot_.Models.Dictionaries
{
    public class Meaning
    {
        [JsonPropertyName("partOfSpeech")]
        public string PartOfSpeech { get; set; }

        [JsonPropertyName("definitions")]
        public List<Definition> Definitions { get; set; }
    }
}

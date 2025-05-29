using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WPFBookStore.Models
{
    public class Book
    {
        [JsonProperty("id")]
        public int IdBook { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("author")]
        public ClassAutor Author { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("genre")]
        public ClassGenres Genres { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("web_amount")]
        public int WebAmount { get; set; }

        [JsonProperty("publisher")]
        public ClassPublishers Publishers { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("cover")]
        public string Cover { get; set; }

        [JsonProperty("file")]
        public bool File { get; set; }

        [JsonProperty("translators")]
        public string[] Translators { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("isbn")]
        public string ISBN { get; set; }
        public string TextTranslators
        {
            get => Translators is string[] array ? string.Join(", ", array) : "не указано";
            set { }
        }
    }
}
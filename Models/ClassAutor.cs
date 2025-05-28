using Newtonsoft.Json;

namespace WPFBookStore.Models
{
    public class ClassAutor
    {
        [JsonProperty("id")]
        public int IdAutor { get; set; }

        [JsonProperty("first_name")]
        public string FirstNameAutor { get; set; }

        [JsonProperty("last_name")]
        public string LastNameAutor { get; set; }
    }
}
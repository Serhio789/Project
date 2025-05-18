using Newtonsoft.Json;

namespace WPFBookStore.Models
{
    public class ClassGenres
    {
        [JsonProperty("id")]
        public int IdGenres { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}

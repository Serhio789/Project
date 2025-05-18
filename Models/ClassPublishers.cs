using Newtonsoft.Json;

namespace WPFBookStore.Models
{
    public class ClassPublishers
    {
        [JsonProperty("id")]
        public int IdPublishers { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}

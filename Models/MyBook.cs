using Newtonsoft.Json;

namespace WPFBookStore.Models
{
    public class MyBook
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("issue_date")]
        public string IssueDate { get; set; }

        [JsonProperty("return_date")]
        public string ReturnDate { get; set; }

        [JsonProperty("book")]
        public int Book { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("autar")]
        public string Autor { get; set; }

        [JsonProperty("cover")]
        public string Cover { get; set; }

        [JsonProperty("reader")]
        public int Reader { get; set; }

        [JsonProperty("is_web")]
        public bool IsWeb { get; set; }
    }
}
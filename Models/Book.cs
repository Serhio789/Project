namespace WPFBookStore.Models
{
    public class Book
    {
        public int idBook {  get; set; }
        public string Title { get; set; }   
        public ClassAutor Author {  get; set; }
        public int Year { get; set; }
        public ClassGenres Genres { get; set; }
        public int Amount { get; set; }
        public int WebAmount { get; set; }
        public ClassPublishers Publishers { get; set; }
        public string Summary { get; set; }
        public string Cover { get; set; }      
        public bool File {  get; set; }
        public string[] Translators { get; set; }
        public string Language { get; set; }    
        public string ISBN { get; set; }
    }

}

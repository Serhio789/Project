using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFBookStore.Data
{
    public class Book
    {
        public string Title { get; set; }        // название
        public int Yers { get; set; }            // год издания
        public string Identifier { get; set; }   // индефикатор
        public string Authors { get; set; }       // авторы
        public string[] Translators { get; set; } // переводчики
        public string Series { get; set; }        // серия
        public string Language { get; set; }      // язык
        public string Description { get; set; }   // описание
        public string Genre { get; set; }         // жанр
        public string ISBN { get; set; }          // isbn
        public string Cover { get; set; }         // обложка
    }

}

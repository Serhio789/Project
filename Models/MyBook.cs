using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFBookStore.Models
{
    public class MyBook
    {
        public int Id { get; set; }
        public string IssueDate { get; set; }
        public string ReturnDate { get; set; }
        public int Book {  get; set; }
        public string Title { get; set; }
        public string Autor {  get; set; }
        public string Cover { get; set; }
        public int Reader { get; set; }
        public bool IsWeb {  get; set; }
    }
}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFBookStore.Pages
{
    /// <summary>
    /// </summary>
    public partial class Dashboard : Page
    {
        public Dashboard()
        {
            InitializeComponent();
            //ChangeLabelContent(); //Запуск для отображения названия книжки
            //Show_Number_Page();     //Запуск для отображения страниц
            //Show_Text_From_Book();  //Запуск для отображения текста книжки
        }

        public static int id;
        public static string bookName="";
        public static string authorName="";
        public static decimal price = decimal.Zero; 
        public static int quantity = 0;
        public static string category = "";
        public static string image = "";
        public static string description = "";

        //Оставил много калла, это всё не используется заисключением GetItemList, для жизни Page_Loaded, можно как-то меня GetItemList.

        private void GetItemList()
        {
            var json = File.ReadAllText(@"Products.json");
            var jObject = JObject.Parse(json);
            if (jObject != null)
            {
                JArray products = (JArray)jObject["Products"];
                if (products != null)
                {
                    List<Models.Addbook> abook = new List<Models.Addbook>()
                    {

                    };

                    foreach (var ibooks in products)
                    {
                        abook.Add(new Models.Addbook() { Id = Convert.ToInt32(ibooks["Id"]), 
                            BookName = ibooks["BookName"].ToString(), 
                            AuthorName = ibooks["AuthorName"].ToString(), 
                            Price = Convert.ToDecimal(ibooks["Price"]), 
                            Quantity = Convert.ToInt32(ibooks["Quantity"]), 
                            Category = ibooks["Category"].ToString(), 
                            Image = ibooks["Image"].ToString(), 
                            Description = ibooks["Description"].ToString() 
                        
                        });
                    }

                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GetItemList();
        }


        //Название книги
        private void ChangeLabelContent() 
        {
            NameBook.Content = "Джордан";
        }

        //Страницы
        private void Show_Number_Page() 
        {
            NumberPageFromBook.Content = "HELP";
        }

        //Текст книги
        private void Show_Text_From_Book() 
        {
            TextFromBook.Text = "HELP_HELP_HELP_HELP_HELP_HELP_HELP_HELP_HELP_HELP_HELP_HELP_HELP";
        }
    }
}

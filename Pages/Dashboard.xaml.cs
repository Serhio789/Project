using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

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
            // Вывод списка книг
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

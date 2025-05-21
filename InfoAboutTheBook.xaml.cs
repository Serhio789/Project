using System.Windows;
using System.Net.Http;
using WPFBookStore.Models;
using WPFBookStore.Data;
using System;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WPFBookStore
{
    public partial class InfoAboutTheBook : Window
    {
        private readonly Book _book;
        private readonly ApiClient _apiClient;
        private bool chekBook = true;
        private Task<Book> book;

        public InfoAboutTheBook(Book book)
        {
            InitializeComponent();
            _book = book;
            DataContext = _book;
            _apiClient = new ApiClient();
            InitializeButtonState();
            InitializeTranslators(book);
        }

        public InfoAboutTheBook(Task<Book> book)
        {
            this.book = book;
        }

        private async void InitializeButtonState()
        {
            try
            {
                List<MyBook> myBooks;
                myBooks = await _apiClient.GetMyBooksAsync();
                foreach (MyBook myBook in myBooks) 
                    if (myBook.Id == _book.IdBook) 
                        chekBook = true;
                ActionButton.Content = chekBook ? "Читать" : "Добавить";
            }
            catch
            {
                ActionButton.Content = "Добавить";
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (ActionButton.Content.ToString() == "Читать")
            {
                MessageBox.Show("Режим чтения не реализован");
                return;
            }

            try
            {
                await _apiClient.TakeBookAsync(_book.IdBook);
                ActionButton.Content = "Читать";
            }
            catch (HttpRequestException ex)
            {
                if (ex.Message.Contains("409"))
                {
                    ActionButton.Content = "Читать";
                }
                else
                {
                    MessageBox.Show("Не удалось добавить книгу", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {
                MessageBox.Show("Не удалось добавить книгу", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void InitializeTranslators(Book book) 
        {
            //for (int i = 0; i <= book.Translators.Length; i++)
            //    TextTranslators.Text += book.Translators[i].ToString();
        }
    }
}
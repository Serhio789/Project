using System.Windows;
using System.Net.Http;
using WPFBookStore.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Data;
using System.Globalization;

namespace WPFBookStore
{
    public partial class InfoAboutTheBook : Window
    {
        private readonly Book _book;
        private readonly ApiClient _apiClient;
        private bool chekBook = false;

        public InfoAboutTheBook(Book book)
        {
            InitializeComponent();
            _book = book;
            DataContext = _book;
            _apiClient = new ApiClient();
            InitializeButtonState();
            InitializeTranslators(book);
            WindowState = WindowState.Maximized;
        }

        private async void InitializeButtonState()
        {
            try
            {
                List<MyBook> myBooks; 
                myBooks = await _apiClient.GetMyBooksAsync();
                foreach (MyBook myBook in myBooks)
                {
                    if (myBook.Title == _book.Title)
                    {
                        chekBook = true;
                    }
                }
                ActionButton.Content = chekBook ? "Вернуть" : "Добавить";
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
            if (ActionButton.Content.ToString() == "Вернуть")
            {
                //var dashboard = new Dashboard(_book);
                //dashboard.Owner = Window.GetWindow(this);
                //dashboard.ShowDialog();
                //return;
            }
            

            try
            {
                //await _apiClient.TakeBookAsync(_book.IdBook);
                //ActionButton.Content = "Вернуть";
            }
            catch (HttpRequestException ex)
            {
                if (ex.Message.Contains("409"))
                {
                    ActionButton.Content = "Вернуть";
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

        private void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            //Кнопка для чтения книги
            Window Dashboard = new Window();
            Dashboard.Show();
        }
    }



    //Нечто Похожее на конверт-конченный
    public class ArrayToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string[] array ? string.Join(", ", array) : "не указано";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
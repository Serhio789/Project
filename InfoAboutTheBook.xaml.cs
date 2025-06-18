using System.Windows;
using System.Net.Http;
using WPFBookStore.Models;
using System.Collections.Generic;
using WPFBookStore.Pages;

namespace WPFBookStore
{
    public partial class InfoAboutTheBook : Window
    {
        private readonly Book _book;
        private readonly ApiClient _apiClient;
        private bool chekBook = false;
        private MyBook _myBook;

        public InfoAboutTheBook(Book book)
        {
            InitializeComponent();
            _book = book;
            DataContext = _book;
            _apiClient = new ApiClient();
            _myBook = new MyBook();
            InitializeButtonState();
            WindowState = WindowState.Maximized;
        }

        private async void InitializeButtonState()
        {
            try
            {
                if (_book.File)
                {
                    List<MyBook> myBooks;
                    myBooks = await _apiClient.GetMyBooksAsync();
                    foreach (MyBook myBook in myBooks)
                    {
                        if (myBook.Title == _book.Title)
                        {
                            _myBook = myBook;
                            chekBook = true;
                        }
                    }
                    ActionButton.Content = chekBook ? "Вернуть" : "Добавить";
                }
                else
                    ActionButton.Content = "Книги нет в наличии";
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
            if (ActionButton.Content == "Вернуть")
            {
                try
                {
                    await _apiClient.ReturnBookAsync(_myBook.Id);
                    _myBook = null;
                    ActionButton.Content = "Добавить";
                }
                catch
                {
                    MessageBox.Show("Не удалось вернуть книгу", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (ActionButton.Content == "Добавить")
            {
                try
                {
                    await _apiClient.TakeBookAsync(_book.IdBook);
                    List<MyBook> myBooks;
                    myBooks = await _apiClient.GetMyBooksAsync();
                    foreach (MyBook myBook in myBooks)
                    {
                        if (myBook.Title == _book.Title)
                        {
                            _myBook = myBook;
                        }
                    }
                    ActionButton.Content = "Вернуть";
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
        }

        private void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            //Кнопка для чтения книги
            if (chekBook == true)
            {
                var dashboard = new Dashboard(_book);
                dashboard.Owner = Window.GetWindow(this);
                dashboard.ShowDialog();
                return;
            }
            else
                MessageBox.Show("Перед прочтением книги нужно ее взять!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
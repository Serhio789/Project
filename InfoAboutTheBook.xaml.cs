using System.Windows;
using System.Net.Http;
using WPFBookStore.Models;
using WPFBookStore.Data;

namespace WPFBookStore
{
    public partial class InfoAboutTheBook : Window
    {
        private readonly Book _book;
        private readonly BookService _bookService;

        public InfoAboutTheBook(Book book)
        {
            InitializeComponent();
            _book = book;
            _bookService = new BookService(); // Используем конструктор без параметров
            DataContext = _book;
            InitializeButtonState();
        }

        private async void InitializeButtonState()
        {
            try
            {
                bool isAdded = await _bookService.IsBookAddedAsync(_book.idBook);
                ActionButton.Content = isAdded ? "Читать" : "Добавить";
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
                await _bookService.AddBookAsync(_book.idBook);
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

    }
}
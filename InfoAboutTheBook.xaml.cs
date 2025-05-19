using System.Windows;
using System.Windows.Controls;
using System.Net.Http;
using WPFBookStore.Models;
using WPFBookStore.Data;

namespace WPFBookStore
{
    public partial class InfoAboutTheBook : Window
    {
        private readonly Book _book;
        private readonly BookService _bookService;
        private readonly ApiClient _apiClient;

        public InfoAboutTheBook(Book book)
        {
            InitializeComponent();
            _book = book;
            _apiClient = new ApiClient();
            //_bookService = new BookService(null); // Упрощаем инициализацию
            _bookService = new BookService(); // Используем упрощенный конструктор
            DataContext = _book;
            InitializeButtonState();
        }


        // Куда добавлять я ещё не вписал так что на добавку можно пока забить!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
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
            this.Close(); // Закрываем окно вместо навигации
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
                await  _apiClient.TakeBookAsync(_book.idBook);
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
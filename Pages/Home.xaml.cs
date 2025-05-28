using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFBookStore.Data;
using WPFBookStore.Models;

namespace WPFBookStore.Pages
{
    public partial class Home : Page
    {
        private readonly ApiClient _client;
        private readonly BookService _bookService;
        public Home()
        {
            InitializeComponent();
            _client = new ApiClient();
            _bookService = new BookService(new Logger<BookService>(new LoggerFactory()));
            Loaded += LoadedMyBook;
            
        }
        private async void LoadedMyBook(object sender, RoutedEventArgs e)
        {
            await LoadGenresAsync();
            await LoadMyBooksAsync();
        }
        private async Task LoadGenresAsync()
        {
            try
            {
                var genres = await _bookService.GetGenresAsync();
            }
            catch
            {
                MessageBox.Show("Ошибка загрузки жанров");
            }
        }

        private async Task LoadMyBooksAsync()
        {
            try
            {
                var books = await _client.GetMyBooksAsync();
                BooksItemsControl.ItemsSource = books;
                tbErrorMessage.Visibility = books?.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }
            catch
            {
                tbErrorMessage.Visibility = Visibility.Visible;
            }
        }

        private async void BookItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((Border)sender).DataContext is MyBook myBook)
            {
                var book = await _bookService.GetBookAsync(myBook.Book);

                var _book = await _bookService.GetBookAsync(book.IdBook);
                var infoWindow = new InfoAboutTheBook(_book);
                infoWindow.Owner = Window.GetWindow(this);
                infoWindow.ShowDialog();
            }
            else
                Debug.WriteLine("jf");
        }
    }
}

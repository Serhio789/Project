using System.Diagnostics.Metrics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFBookStore.Data;
using WPFBookStore.Models;

namespace WPFBookStore.Pages
{
    /// <summary>
    ///Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        private ApiClient _client;
        public Home()
        {
            InitializeComponent();
            _client = new ApiClient();
            Loaded += LoadedMyBook;
        }

        private async void LoadedMyBook(object sender, RoutedEventArgs e)
        {
            await LoadMyBooksAsync();
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

        private void MySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MyScrollViewer != null)
            {
                MyScrollViewer.ScrollToVerticalOffset(e.NewValue);
            }
        }
        private void BookItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((Border)sender).DataContext is Book book)
            {
                // Открываем новое окно вместо навигации
                var infoWindow = new InfoAboutTheBook(book);
                infoWindow.Owner = Window.GetWindow(this);
                infoWindow.ShowDialog();
            }
        }
    }
}

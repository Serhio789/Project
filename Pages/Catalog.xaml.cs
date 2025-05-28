using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFBookStore.Data;
using WPFBookStore.Models;
using System.Windows.Input;
using System.Diagnostics;

namespace WPFBookStore.Pages
{
    public partial class Catalog : Page
    {
        private readonly BookService _bookService;

        public Catalog()
        {
            InitializeComponent();
            _bookService = new BookService(new Logger<BookService>(new LoggerFactory()));
            Loaded += Catalog_Loaded;
        }

        private async void Catalog_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadGenresAsync();
            await LoadAllBooksAsync(); // Загрузка всех книг при открытии
        }

        private async Task LoadGenresAsync()
        {
            try
            {
                var genres = await _bookService.GetGenresAsync();
                cmbGenre.ItemsSource = genres;
            }
            catch
            {
                MessageBox.Show("Ошибка загрузки жанров");
            }
        }

        private async Task LoadAllBooksAsync()
        {
            try
            {
                var books = await _bookService.GetListBooksAsync();
                BooksItemsControl.ItemsSource = books;
                tbErrorMessage.Visibility = books?.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }
            catch
            {
                tbErrorMessage.Visibility = Visibility.Visible;
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var title = GetCleanText(txtTitle.Text, "Название");
            var genre = (cmbGenre.SelectedItem as ClassGenres)?.IdGenres;
            var year = ParseYear(txtYear.Text);
            var author = GetCleanText(txtAuthor.Text, "Автор");

            try
            {
                var books = await _bookService.GetListBooksAsync(title, genre, year, author);
                BooksItemsControl.ItemsSource = books;
                tbErrorMessage.Visibility = books?.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }
            catch
            {
                tbErrorMessage.Visibility = Visibility.Visible;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            // Сброс фильтров
            txtTitle.Text = "Название";
            txtTitle.Foreground = System.Windows.Media.Brushes.Gray;

            cmbGenre.SelectedIndex = -1;

            txtYear.Text = "Год";
            txtYear.Foreground = System.Windows.Media.Brushes.Gray;

            txtAuthor.Text = "Автор";
            txtAuthor.Foreground = System.Windows.Media.Brushes.Gray;

            LoadAllBooksAsync(); // Повторная загрузка всех книг
        }

        // Вспомогательные методы
        private string GetCleanText(string text, string placeholder)
            => text == placeholder ? null : text;

        private int? ParseYear(string text)
            => int.TryParse(text, out var y) && y > 0 ? y : (int?)null;

        // Обработчики плейсхолдеров
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = (TextBox)sender;
            if (tb.Text == "Название" || tb.Text == "Год" || tb.Text == "Автор")
            {
                tb.Text = "";
                tb.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                if (tb.Name == "txtTitle") tb.Text = "Название";
                else if (tb.Name == "txtYear") tb.Text = "Год";
                else if (tb.Name == "txtAuthor") tb.Text = "Автор";
                tb.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private async void BookItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((Border)sender).DataContext is Book book)
            {
                var _book = await _bookService.GetBookAsync(book.IdBook);
                var infoWindow = new InfoAboutTheBook(_book);
                infoWindow.Owner = Window.GetWindow(this);
                infoWindow.ShowDialog();
            }
        }
    }
}
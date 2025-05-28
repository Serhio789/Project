using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using WPFBookStore.Data;
using WPFBookStore.Models;

namespace WPFBookStore.Pages
{
    /// <summary>
    /// Логика взаимодействия для Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        private readonly BookTextClient _bookClient;
        private BookTextClient.BookPageNavigation _currentBook;
        private readonly Book _book;

        public Dashboard(Book book)
        {
            InitializeComponent();
            _bookClient = new BookTextClient();
            _book = book;
            // Инициализация состояния кнопок
            PrevPageBtn.IsEnabled = false;
            NextPageBtn.IsEnabled = false;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Инициализация данных книги
            //NameBook.Content = BookName;
            LoadBook(); // Автозагрузка при открытии
        }

        private async void LoadBook()
        {
            try
            {
                _currentBook = await _bookClient.LoadBookWithNavigation(
                    _book.IdBook,
                    ContentTextBox.FontFamily,
                    ContentTextBox.FontSize,
                    ContentScrollViewer.ActualWidth,
                    ContentScrollViewer.ActualHeight);

                UpdatePageDisplay();
                UpdateNavigationButtons();
                NameBook.Text = _book.Title; // Название книги
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки книги: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdatePageDisplay()
        {
            if (_currentBook == null) return;

            ContentTextBox.Text = _currentBook.CurrentPageContent;
            PageInfoText.Text = $"Страница {_currentBook.CurrentPageIndex + 1} из {_currentBook.TotalPages}";
            PageNumberBox.Text = (_currentBook.CurrentPageIndex + 1).ToString();
            NumberPageFromBook.Text = $"Страница {_currentBook.CurrentPageIndex + 1}";

            ContentScrollViewer.ScrollToTop();
        }

        private void UpdateNavigationButtons()
        {
            if (_currentBook == null) return;

            PrevPageBtn.IsEnabled = _currentBook.CurrentPageIndex > 0;
            NextPageBtn.IsEnabled = _currentBook.CurrentPageIndex < _currentBook.TotalPages - 1;
        }

        private void PrevPageBtn_Click(object sender, RoutedEventArgs e) => NavigatePage(-1);
        private void NextPageBtn_Click(object sender, RoutedEventArgs e) => NavigatePage(1);

        private void NavigatePage(int direction)
        {
            if (_currentBook == null) return;

            _currentBook = direction > 0
                ? _bookClient.GoToNextPage()
                : _bookClient.GoToPreviousPage();

            UpdatePageDisplay();
            UpdateNavigationButtons();
        }

        private void GoToPageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentBook == null || !int.TryParse(PageNumberBox.Text, out int pageNumber)) return;

            try
            {
                _currentBook = _bookClient.GoToPage(pageNumber - 1);
                UpdatePageDisplay();
                UpdateNavigationButtons();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

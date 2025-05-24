using System;
using System.Windows;
using System.Windows.Controls;
using WPFBookStore.Data;

namespace WPFBookStore.Pages
{
    public partial class Dashboard : Page
    {
        private readonly BookTextClient _bookClient;
        private BookTextClient.BookPageNavigation _currentBook;

        public static int BookId { get; set; }
        public static string BookName { get; set; }

        public Dashboard()
        {
            InitializeComponent();
            _bookClient = new BookTextClient();

            // Инициализация состояния кнопок
            PrevPageBtn.IsEnabled = false;
            NextPageBtn.IsEnabled = false;
        }

        private async void LoadBookBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _currentBook = await _bookClient.LoadBookWithNavigation(
                    BookId,
                    ContentTextBox.FontFamily,
                    ContentTextBox.FontSize,
                    ContentScrollViewer.ActualWidth,
                    ContentScrollViewer.ActualHeight);

                UpdatePageDisplay();
                UpdateNavigationButtons();
                //NameBook.Content = BookName; // Название книги
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Инициализация данных книги
            //NameBook.Content = BookName;
            LoadBookBtn_Click(null, null); // Автозагрузка при открытии
        }
    }
}
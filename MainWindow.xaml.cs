using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Navigation;

namespace WPFBookStore
{
    public partial class MainWindow : Window
    {
        private readonly ApiClient _apiClient;
        private bool _isDragging;
        private Point _startPoint;

        public MainWindow(ApiClient apiClient)
        {
            InitializeComponent();
            _apiClient = apiClient;
            InitializeEventHandlers();
            LoadUserData();
            InitializeNavigation();
        }

        private void InitializeEventHandlers()
        {
            // Обработчики для перемещения окна
            MouseLeftButtonDown += (s, e) =>
            {
                if (e.GetPosition(this).Y < 50) // Перемещение только за верхнюю часть
                {
                    _isDragging = true;
                    _startPoint = e.GetPosition(this);
                    CaptureMouse();
                }
            };

            MouseLeftButtonUp += (s, e) =>
            {
                _isDragging = false;
                ReleaseMouseCapture();
            };

            MouseMove += (s, e) =>
            {
                if (_isDragging)
                {
                    Point currentPosition = PointToScreen(e.GetPosition(this));
                    Left = currentPosition.X - _startPoint.X;
                    Top = currentPosition.Y - _startPoint.Y;
                }
            };

            // Обработчик изменения состояния окна
            StateChanged += (s, e) =>
            {
                if (WindowState == WindowState.Maximized)
                {
                    BorderThickness = new Thickness(7);
                }
                else
                {
                    BorderThickness = new Thickness(0);
                }
            };
        }

        private async void LoadUserData()
        {
            try
            {
                var accountData = await _apiClient.GetAccountDataAsync();
                if (!string.IsNullOrEmpty(accountData))
                {
                    // Здесь можно обновить UI с данными пользователя
                    // Например: UserName.Text = accountData.UserName;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void InitializeNavigation()
        {
            fContainer.Navigated += (s, e) =>
            {
                // Скрываем навигационную панель после загрузки страницы
                Tg_Btn.IsChecked = false;
            };

            // Загружаем стартовую страницу
            NavigateToPage("Pages/Home.xaml");
        }

        private void NavigateToPage(string pageUri)
        {
            try
            {
                fContainer.Navigate(new Uri(pageUri, UriKind.RelativeOrAbsolute));
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка загрузки страницы: {ex.Message}");
            }
        }

        private void ShowErrorMessage(string message)
        {
            // Можно реализовать красивый Toast или модальное окно
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        #region Обработчики кнопок навигации
        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage("Pages/Home.xaml");
        }

        private void btnCatalog_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage("Pages/Catalog.xaml");
        }

        private void btnExitFromAccount_Click(object sender, RoutedEventArgs e)
        {
            _apiClient.Logout();
            Close();
        }
        #endregion

        #region Обработчики управления окном
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Normal? WindowState.Maximized: WindowState.Normal;
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        #endregion

        #region Обработчики всплывающих подсказок
        private void btnHome_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowPopup(btnHome, "Личный Кабинет");
        }

        private void btnCatalog_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowPopup(btnCatalog, "Каталог");
        }

        private void btnExitFromAccount_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowPopup(btnExitFromAccount, "Выход из аккаунта");
        }

        private void ShowPopup(Button target, string text)
        {
            if (Tg_Btn.IsChecked == false)
            {
                Popup.PlacementTarget = target;
                Popup.Placement = PlacementMode.Right;
                Header.PopupText.Text = text;
                Popup.IsOpen = true;
            }
        }

        private void btnHome_MouseLeave(object sender, MouseEventArgs e)
        {
            HidePopup();
        }

        private void btnDashboard_MouseLeave(object sender, MouseEventArgs e)
        {
            HidePopup();
        }

        private void btnCatalog_MouseLeave(object sender, MouseEventArgs e)
        {
            HidePopup();
        }

        private void btnExitFromAccount_MouseLeave(object sender, MouseEventArgs e)
        {
            HidePopup();
        }

        private void HidePopup()
        {
            Popup.IsOpen = false;
        }
        #endregion

        private void fContainer_Navigated(object sender, NavigationEventArgs e)
        {
        }

        #region Отступы и максимизация
        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            if (WindowState == WindowState.Maximized)
            {
                // Обновление элементов при максимизации
                mainContent.Margin = new Thickness(0);
            }
            else
            {
                // Возвращение отступов в нормальное состояние
                mainContent.Margin = new Thickness(0);
            }
        }

        #endregion

    }
}
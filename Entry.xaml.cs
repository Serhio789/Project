using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Net.Http;

namespace WPFBookStore
{
    public partial class Entry : Window
    {
        private readonly ApiClient _apiClient = new ApiClient();
        private bool _isDragging = false;
        private Point _startPoint;

        public Entry()
        {
            InitializeComponent();
            InitializeEventHandlers();
            if (_apiClient.GetAuthToken() != "")
                OpenMainWindow();
            CheckAutoLogin();
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
                }
            };

            MouseLeftButtonUp += (s, e) => _isDragging = false;
            MouseMove += (s, e) =>
            {
                if (_isDragging)
                {
                    Point currentPosition = e.GetPosition(this);
                    Left += currentPosition.X - _startPoint.X;
                    Top += currentPosition.Y - _startPoint.Y;
                }
            };

            // Обработчики для текстовых полей
            txtUsername.GotFocus += (s, e) =>
            {
                if (txtUsername.Text == "Логин")
                {
                    txtUsername.Text = "";
                    txtUsername.Foreground = System.Windows.Media.Brushes.Black;
                }
            };

            txtUsername.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text))
                {
                    txtUsername.Text = "Логин";
                    txtUsername.Foreground = System.Windows.Media.Brushes.Gray;
                }
            };

            txtPassword.GotFocus += (s, e) =>
            {
                if (txtPassword.Text == "Пароль")
                {
                    txtPassword.Text = "";
                    txtPassword.Foreground = System.Windows.Media.Brushes.Black;
                }
            };

            txtPassword.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    txtPassword.Text = "Пароль";
                    txtPassword.Foreground = System.Windows.Media.Brushes.Gray;
                }
            };
        }

        private async void CheckAutoLogin()
        {
            if (_apiClient.GetAuthToken() != "")
            {
                try
                {
                    SetLoadingState(true);
                    string accountData = await _apiClient.GetAccountDataAsync();
                    if (!string.IsNullOrEmpty(accountData))
                    {
                        OpenMainWindow();
                    }
                }
                catch
                {
                    // Автоматический вход не удался
                }
                finally
                {
                    SetLoadingState(false);
                }
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text == "Логин" ? "" : txtUsername.Text;
            string password = txtPassword.Text == "Пароль" ? "" : txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ShowErrorMessage("Введите имя пользователя и пароль");
                return;
            }

            SetLoadingState(true);
            btnLogin.IsEnabled = false;

            try
            {
                bool success = await _apiClient.LoginAsync(username, password);

                if (success)
                {
                    OpenMainWindow();
                }
                else
                {
                    ShowErrorMessage("Неверные учетные данные");
                }
            }
            catch (UnauthorizedAccessException)
            {
                ShowErrorMessage("Ошибка авторизации");
            }
            catch (HttpRequestException ex)
            {
                ShowErrorMessage($"Ошибка сети: {ex.Message}");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка: {ex.Message}");
            }
            finally
            {
                SetLoadingState(false);
                btnLogin.IsEnabled = true;
            }
        }

        private void OpenMainWindow()
        {
            var mainWindow = new MainWindow(_apiClient);
            mainWindow.Show();
            this.Close();
        }

        private void SetLoadingState(bool isLoading)
        {
            if (isLoading)
            {
                btnLogin.Content = "Загрузка...";
                btnLogin.IsEnabled = false;
                btnExit.IsEnabled = false;
            }
            else
            {
                btnLogin.Content = "Вход";
                btnLogin.IsEnabled = true;
                btnExit.IsEnabled = true;
            }
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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
using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using WpfApplicationSysTech1.app;
using WpfApplicationSysTech1.app.repo;

namespace WpfApplicationSysTech1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly AppMain _appMain;

        public MainWindow()
        {
            InitializeComponent();
            _appMain = AppMain.Instance;
        }

        private void ResetBlockControlsState()
        {
            BlockControlsBorder.Visibility = Visibility.Hidden;
            BlockControlsTextBlock.Text = Const.DefaultLoadingMessage;
        }

        private void ShowBlockControlsBorder(string text = Const.DefaultLoadingMessage)
        {
            BlockControlsBorder.Visibility = Visibility.Visible;
            BlockControlsTextBlock.Text = text;
        }

        // Переход на окно регистрации
        private void MoveToRegWindowClickHandler(object sender, RoutedEventArgs e)
        {
            var regWindow = new RegWindow();
            regWindow.Show();
            Close();
        }

        // Проверка на то, что это авторизуется администратор или нет
        private bool IsAdminLogin()
        {
            return TextBoxLogin.Text == _appMain.AppSettings.AdminLogin &&
                   PasswordBox.Password == _appMain.AppSettings.AdminPassword;
        }

        private void GoAwayFromAuthWindow()
        {
            // Определяем куда можно перейти
            if (_appMain.IsAdmin || _appMain.IsModer)
            {
                // Переход к списку сотрудников
                var usersWindow = new UsersListWindow();
                usersWindow.Show();
                Close();
            }
            else
            {
                // Переход к профилю пользователя текущего
                var userWindow = new MyProfileWindow();
                userWindow.Show();
                Close();
            }
        }

        // Проверка введенных пользователем данных авторизации
        private void CheckUserCredentials(string login, string password)
        {
            _appMain.LoadPositionsArr();

            // Определить роль пользователи и переместить его на нужно окно
            var existingUser = UsersRepo.GetUserByCredentials(login, password);
            if (existingUser != null && existingUser.IsActive == 1)
            {
                // Авторизовались
                _appMain.SetLoginInAppAsUser(existingUser);

                // Определяем куда можно перейти
                Dispatcher.Invoke(GoAwayFromAuthWindow);

                //MessageBox.Show(Properties.Resources.SuccessAuthText, Const.AppName);
                Console.WriteLine(Properties.Resources.SuccessAuthText);
            }
            else
            {
                // Ошибка авторизации
                MessageBox.Show(Properties.Resources.AuthErrorText, Const.AppName);
                Console.WriteLine(Properties.Resources.AuthErrorText);
            }

            Dispatcher.Invoke(ResetBlockControlsState);
        }

        // Обработка авторизации как администратора
        private void AdminLoginHandler()
        {
            _appMain.LoadPositionsArr();
            // Установим для приложения, что мы администратор
            _appMain.SetLoginInAppAsAdmin();

            Dispatcher.Invoke(() =>
            {
                ResetBlockControlsState();

                // Переход к списку сотрудников
                var usersWindow = new UsersListWindow();
                usersWindow.Show();
                Close();
            });
        }

        private void AuthButtonClickHandler(object sender, RoutedEventArgs e)
        {
            if (IsAdminLogin())
            {
                ShowBlockControlsBorder();
                new Thread(AdminLoginHandler).Start();

                return;
            }

            // Иначе проверка пользователя на корректность ввода
            string login = TextBoxLogin.Text.Trim();
            string password = PasswordBox.Password.Trim();

            bool isNoErrorsFound = true;

            if (login.Length < Const.MinFieldLength)
            {
                TextBoxLogin.ToolTip = Const.ErrorTooltip;
                TextBoxLogin.Background = Brushes.Crimson;
                isNoErrorsFound = false;
            }
            else
            {
                TextBoxLogin.ToolTip = string.Empty;
                TextBoxLogin.Background = Brushes.Transparent;
            }

            if (password.Length < Const.MinFieldLength)
            {
                PasswordBox.ToolTip = Const.ErrorTooltip;
                PasswordBox.Background = Brushes.Crimson;
                isNoErrorsFound = false;
            }
            else
            {
                PasswordBox.ToolTip = string.Empty;
                PasswordBox.Background = Brushes.Transparent;
            }

            if (isNoErrorsFound)
            {
                ShowBlockControlsBorder();
                new Thread(() => CheckUserCredentials(login, password)).Start();
            }
        }
    }
}

using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using WpfApplicationSysTech1.app;
using WpfApplicationSysTech1.app.models;
using WpfApplicationSysTech1.app.repo;

namespace WpfApplicationSysTech1
{
    public partial class MyProfileWindow
    {
        private readonly AppMain _appMain;

        public MyProfileWindow()
        {
            InitializeComponent();

            _appMain = AppMain.Instance;

            if (!_appMain.IsAdmin && !_appMain.IsModer)
            {
                // Скрываем инструменты модератора, администратора
                SaveMyLoginButton.Visibility = Visibility.Hidden;
                MyLoginTextBox.IsReadOnly = true;
            }

            if (_appMain.IsAdmin)
            {
                MySalaryOnDateButton.Visibility = Visibility.Collapsed;
                MySubsButton.Visibility = Visibility.Collapsed;

                SaveMyLoginButton.Visibility = Visibility.Hidden;
                SaveMyNameButton.Visibility = Visibility.Hidden;
                SaveMyNewPasswordButton.Visibility = Visibility.Hidden;

                PasswordStackPanel.Visibility = Visibility.Hidden;

                MyLoginTextBox.IsReadOnly = true;
                MyNameTextBox.IsReadOnly = true;
            }

            Position myPos = _appMain.Positions.FirstOrDefault(p => p.Id == _appMain.CurrentUser.PositionId);
            if ((myPos == null || myPos.IsCanHaveSubs == 0) && !_appMain.IsAdmin)
            {
                // Скрываем инструменты по поводу подчиненных
                MySubsButton.Visibility = Visibility.Collapsed;
            }

            MyNameTextBox.Text = _appMain.CurrentUser.Name;
            MyLoginTextBox.Text = _appMain.CurrentUser.Login;
            IncomeDateTextBlock.Text = _appMain.CurrentUser.IncomeDate;
            MyWorkRoleTextBlock.Text =
                _appMain.Positions.FirstOrDefault(p => p.Id == _appMain.CurrentUser.PositionId)?.Name;

            // Только не у администратора может быть начальник
            if (!_appMain.IsAdmin)
            {
                ShowBlockControlsBorder();
                new Thread(GetMyBossName).Start();
            }
        }

        // Получить имя начальника
        private void GetMyBossName()
        {
            var myBossUser = UsersRepo.FindUser(_appMain.CurrentUser.BossId);
            if (myBossUser != null)
            {
                Dispatcher.Invoke(() =>
                {
                    MyBossNameTextBlock.Text = myBossUser.Name;
                });
            }

            Dispatcher.Invoke(ResetBlockControlsState);
        }

        // Скрыть блокирующий блок
        private void ResetBlockControlsState()
        {
            BlockControlsBorder.Visibility = Visibility.Hidden;
            BlockControlsTextBlock.Text = Const.DefaultLoadingMessage;
        }

        // Показ блокирующего блока (блокирует взаимодействие)
        private void ShowBlockControlsBorder(string text = Const.DefaultLoadingMessage)
        {
            BlockControlsBorder.Visibility = Visibility.Visible;
            BlockControlsTextBlock.Text = text;
        }

        // Выйти и перейти на авторизацию
        private void LogoutButtonClickHandler(object sender, RoutedEventArgs e)
        {
            _appMain.ResetCurrentUser();

            var authWindow = new MainWindow();
            authWindow.Show();
            Close();
        }

        // Перейти к списку пользователей
        private void ToUsersListClickHandler(object sender, RoutedEventArgs e)
        {
            var usersWindow = new UsersListWindow();
            usersWindow.Show();
            Close();
        }

        // Сохранить моё новое имя
        private void SaveMyNameButtonClickHandler(object sender, RoutedEventArgs e)
        {
            string newName = MyNameTextBox.Text;

            if (!string.IsNullOrEmpty(newName) && newName.Length >= Const.MinFieldLength)
            {
                MyNameTextBox.ToolTip = string.Empty;
                MyNameTextBox.Background = Brushes.Transparent;

                ShowBlockControlsBorder();
                new Thread(() => { SetNewName(newName); }).Start();
            }
            else
            {
                MyNameTextBox.ToolTip = Const.ErrorTooltip;
                MyNameTextBox.Background = Brushes.Crimson;
            }
        }

        private void SetNewName(string name)
        {
            UsersRepo.SetNewNameToUser(_appMain.CurrentUser.Id, name);
            Dispatcher.Invoke(ResetBlockControlsState);
        }

        // Сохранить мой логин
        private void SaveMyLoginButtonClickHandler(object sender, RoutedEventArgs e)
        {
            string newLogin = MyLoginTextBox.Text;

            if (!string.IsNullOrEmpty(newLogin) && newLogin.Length >= Const.MinFieldLength)
            {
                MyLoginTextBox.ToolTip = string.Empty;
                MyLoginTextBox.Background = Brushes.Transparent;

                ShowBlockControlsBorder();
                new Thread(() => { SetNewLogin(newLogin); }).Start();
            }
            else
            {
                MyLoginTextBox.ToolTip = Const.ErrorTooltip;
                MyLoginTextBox.Background = Brushes.Crimson;
            }
        }

        private void SetNewLogin(string login)
        {
            UsersRepo.SetNewLoginToUser(_appMain.CurrentUser.Id, login);
            Dispatcher.Invoke(ResetBlockControlsState);
        }

        // Сохранить мой новый пароль
        private void SaveMyNewPasswordButtonClickHandler(object sender, RoutedEventArgs e)
        {
            string newPassword = MyNewPasswordBox.Password;

            if (!string.IsNullOrEmpty(newPassword) && newPassword.Length >= Const.MinFieldLength)
            {
                MyNewPasswordBox.ToolTip = string.Empty;
                MyNewPasswordBox.Background = Brushes.Transparent;

                ShowBlockControlsBorder();
                new Thread(() => { SetNewPassword(newPassword); }).Start();
            }
            else
            {
                MyNewPasswordBox.ToolTip = Const.ErrorTooltip;
                MyNewPasswordBox.Background = Brushes.Crimson;
            }
        }

        private void SetNewPassword(string password)
        {
            UsersRepo.SetNewPasswordToUser(_appMain.CurrentUser.Id, password);
            Dispatcher.Invoke(ResetBlockControlsState);
            Dispatcher.Invoke(() =>
            {
                MyNewPasswordBox.Password = string.Empty;
            });
        }

        // Мои подчиненные
        private void MySubsButtonClickHandler(object sender, RoutedEventArgs e)
        {
            SelectedUserData.SetSelectedUser(_appMain.CurrentUser);

            var usersWindow = new ListOfSubWorkersWindow();
            usersWindow.Show();
            Close();
        }

        // Моя зарплата
        private void MySalaryOnDateButtonClickHandler(object sender, RoutedEventArgs e)
        {
            SelectedUserData.SetSelectedUser(_appMain.CurrentUser);

            var salaryWindow = new SalaryOnDateWindow();
            salaryWindow.Show();
            Close();
        }
    }
}

using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using WpfApplicationSysTech1.app;
using WpfApplicationSysTech1.app.repo;

namespace WpfApplicationSysTech1
{
    public partial class UserProfileWindow
    {
        private readonly AppMain _appMain;

        public UserProfileWindow()
        {
            InitializeComponent();
            _appMain = AppMain.Instance;

            if (!_appMain.IsAdmin && !_appMain.IsModer)
            {
                NameTextBox.IsReadOnly = true;
                LoginTextBox.IsReadOnly = true;
                NewPasswordBox.IsEnabled = false;

                PasswordTitleTextBlock.Visibility = Visibility.Hidden;
                NewPasswordBox.Visibility = Visibility.Hidden;

                SalaryOnDateButton.Visibility = Visibility.Collapsed;
                ChangeUserRoleButton.Visibility = Visibility.Collapsed;

                SaveNameButton.Visibility = Visibility.Hidden;
                SaveLoginButton.Visibility = Visibility.Hidden;
                SaveNewPasswordButton.Visibility = Visibility.Hidden;

                RemoveUserButton.Visibility = Visibility.Collapsed;
                ActivateUserButton.Visibility = Visibility.Collapsed;
                DeActivateUserButton.Visibility = Visibility.Collapsed;
            }

            NameTextBox.Text = SelectedUserData.SelectedUser.Name;
            LoginTextBox.Text = SelectedUserData.SelectedUser.Login;
            IncomeDateTextBlock.Text = SelectedUserData.SelectedUser.IncomeDate;
            WorkRoleTextBlock.Text = SelectedUserData.SelectedUserListItem.PositionName;

            if (_appMain.IsAdmin || _appMain.IsModer)
            {
                if (SelectedUserData.SelectedUserListItem.IsActive)
                    ActivateUserButton.Visibility = Visibility.Collapsed;
                else
                    DeActivateUserButton.Visibility = Visibility.Collapsed;
            }

            var userPosition = _appMain.Positions.FirstOrDefault(p => p.Id == SelectedUserData.SelectedUser.PositionId);
            if (userPosition != null)
            {
                if (userPosition.IsCanHaveSubs == 0)
                    SubsButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                SubsButton.Visibility = Visibility.Collapsed;
            }

            ShowBlockControlsBorder();
            new Thread(GetBossName).Start();

            // Мы не администратор, а пользователь
            if (!_appMain.IsAdmin)
            {
                // То является ли этот пользователь моим подчиненным?
                if (SelectedUserData.SelectedUser.BossId != _appMain.CurrentUser.Id)
                {
                    Console.WriteLine("Это не наш подчиненный, мы НЕ МОЖЕМ смотреть его ЗП");
                    SalaryOnDateButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Console.WriteLine("Это наш подчиненный, мы МОЖЕМ смотреть его ЗП");
                }
            }
        }

        // Получить имя начальника
        private void GetBossName()
        {
            var myBossUser = UsersRepo.FindUser(SelectedUserData.SelectedUser.BossId);
            if (myBossUser != null)
            {
                Dispatcher.Invoke(() =>
                {
                    BossNameTextBlock.Text = myBossUser.Name;
                });
            }

            Dispatcher.Invoke(ResetBlockControlsState);
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

        private void ToUsersListClickHandler(object sender, RoutedEventArgs e)
        {
            var usersWindow = new UsersListWindow();
            usersWindow.Show();
            Close();
        }

        // Сохранить моё новое имя
        private void SaveNameButtonClickHandler(object sender, RoutedEventArgs e)
        {
            string newName = NameTextBox.Text;

            if (!string.IsNullOrEmpty(newName) && newName.Length >= Const.MinFieldLength)
            {
                NameTextBox.ToolTip = string.Empty;
                NameTextBox.Background = Brushes.Transparent;

                ShowBlockControlsBorder();
                new Thread(() => { SetNewName(newName); }).Start();
            }
            else
            {
                NameTextBox.ToolTip = Const.ErrorTooltip;
                NameTextBox.Background = Brushes.Crimson;
            }
        }

        private void SetNewName(string name)
        {
            UsersRepo.SetNewNameToUser(SelectedUserData.SelectedUser.Id, name);
            Dispatcher.Invoke(ResetBlockControlsState);
        }

        // Сохранить мой логин
        private void SaveLoginButtonClickHandler(object sender, RoutedEventArgs e)
        {
            string newLogin = LoginTextBox.Text;

            if (!string.IsNullOrEmpty(newLogin) && newLogin.Length >= Const.MinFieldLength)
            {
                LoginTextBox.ToolTip = string.Empty;
                LoginTextBox.Background = Brushes.Transparent;

                ShowBlockControlsBorder();
                new Thread(() => { SetNewLogin(newLogin); }).Start();
            }
            else
            {
                LoginTextBox.ToolTip = Const.ErrorTooltip;
                LoginTextBox.Background = Brushes.Crimson;
            }
        }

        private void SetNewLogin(string login)
        {
            UsersRepo.SetNewLoginToUser(SelectedUserData.SelectedUser.Id, login);
            Dispatcher.Invoke(ResetBlockControlsState);
        }

        // Сохранить мой новый пароль
        private void SaveNewPasswordButtonClickHandler(object sender, RoutedEventArgs e)
        {
            string newPassword = NewPasswordBox.Password;

            if (!string.IsNullOrEmpty(newPassword) && newPassword.Length >= Const.MinFieldLength)
            {
                NewPasswordBox.ToolTip = string.Empty;
                NewPasswordBox.Background = Brushes.Transparent;

                ShowBlockControlsBorder();
                new Thread(() => { SetNewPassword(newPassword); }).Start();
            }
            else
            {
                NewPasswordBox.ToolTip = Const.ErrorTooltip;
                NewPasswordBox.Background = Brushes.Crimson;
            }
        }

        private void SetNewPassword(string password)
        {
            UsersRepo.SetNewPasswordToUser(SelectedUserData.SelectedUser.Id, password);
            Dispatcher.Invoke(ResetBlockControlsState);
            Dispatcher.Invoke(() =>
            {
                NewPasswordBox.Password = string.Empty;
            });
        }

        // Переходим к выбору роли для пользователя
        private void ChangUserRoleClickHandler(object sender, RoutedEventArgs e)
        {
            var selectRoleWindow = new SelectNewRoleForUserWindow();
            selectRoleWindow.Show();
            Close();
        }

        private void SubsButtonClickHandler(object sender, RoutedEventArgs e)
        {
            var usersWindow = new ListOfSubWorkersWindow();
            usersWindow.Show();
            Close();
        }

        private void ActivateUserButtonClickHandler(object sender, RoutedEventArgs e)
        {
            ShowBlockControlsBorder();
            new Thread(() =>
            {
                ActivateOrNotUser(1);
            }).Start();
        }

        private void DeActivateUserButtonClickHandler(object sender, RoutedEventArgs e)
        {
            ShowBlockControlsBorder();
            new Thread(() =>
            {
                ActivateOrNotUser(0);
            }).Start();
        }

        private void RemoveUserButtonClickHandler(object sender, RoutedEventArgs e)
        {
            ShowBlockControlsBorder();
            new Thread(RemoveUser).Start();
        }

        private void ActivateOrNotUser(int activateFlag)
        {
            UsersRepo.SetActiveFlagToThisUser(SelectedUserData.SelectedUser.Id, activateFlag);
            Dispatcher.Invoke(() =>
            {
                if (activateFlag == 0)
                {
                    // Теперь можем активировать
                    ActivateUserButton.Visibility = Visibility.Visible;
                    DeActivateUserButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // Теперь можем деактивировать
                    ActivateUserButton.Visibility = Visibility.Collapsed;
                    DeActivateUserButton.Visibility = Visibility.Visible;
                }
                ResetBlockControlsState();
            });
        }

        private void RemoveUser()
        {
            UsersRepo.RemoveThisUser(SelectedUserData.SelectedUser.Id);
            Dispatcher.Invoke(() =>
            {
                ResetBlockControlsState();

                var usersWindow = new UsersListWindow();
                usersWindow.Show();
                Close();
            });
        }

        private void SalaryButtonClickHandler(object sender, RoutedEventArgs e)
        {
            var salaryWindow = new SalaryOnDateWindow();
            salaryWindow.Show();
            Close();
        }
    }
}

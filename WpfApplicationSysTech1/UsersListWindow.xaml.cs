using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using WpfApplicationSysTech1.app;
using WpfApplicationSysTech1.app.models;
using WpfApplicationSysTech1.app.repo;
using WpfApplicationSysTech1.app.types;

namespace WpfApplicationSysTech1
{
    public partial class UsersListWindow
    {
        public UsersListWindow()
        {
            InitializeComponent();

            var appMain = AppMain.Instance;

            ShowBlockControlsBorder();
            new Thread(LoadUsersList).Start();

            if (!appMain.IsAdmin && !appMain.IsModer)
            {
                AddNewUserButton.Visibility = Visibility.Hidden;
            }

            SelectedUserData.ResetSelectedUser();
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

        private void LoadUsersList()
        {
            List<User> usersList = UsersRepo.GetUsersList();
            List<UserListItem> resultList = DataConverter.GetUsersList(usersList, AppMain.Instance.Positions);

            Dispatcher.Invoke(() =>
            {
                UsersListView.ItemsSource = resultList;
                ResetBlockControlsState();
            });
        }

        private void AddNewUserButtonClickHandler(object sender, RoutedEventArgs e)
        {
            var addUserWindow = new AddNewUserWindow();
            addUserWindow.Show();
            Close();
        }

        private void OpenMyProfileClickHandler(object sender, RoutedEventArgs e)
        {
            var userWindow = new MyProfileWindow();
            userWindow.Show();
            Close();
        }

        private void UsersListSelectionChangeHandler(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var item = (sender as ListView)?.SelectedItem;
                if (item is UserListItem userItem)
                {
                    var user = new User
                    {
                        Id = userItem.Id,
                        BossId = userItem.BossId,
                        IncomeDate = userItem.IncomeDateStr,
                        IsActive = userItem.IsActive ? 1 : 0,
                        Login = userItem.Login,
                        Name = userItem.Name,
                        Password = userItem.Password,
                        PositionId = userItem.PositionId
                    };

                    SelectedUserData.SetSelectedUser(user, userItem);

                    // Определить выбрали ли мы сами себя или нет (если мы не администратор, а зашли как пользователь)
                    var isSelfUser = userItem.Id == AppMain.Instance.CurrentUser.Id;
                    if (isSelfUser)
                    {
                        // Мой профиль
                        var myProfileWindow = new MyProfileWindow();
                        myProfileWindow.Show();
                        Close();
                    }
                    else
                    {
                        // Профиль пользователя
                        var userProfileWindow = new UserProfileWindow();
                        userProfileWindow.Show();
                        Close();
                    }
                }
                else
                {
                    Console.WriteLine(Properties.Resources.ThisIsNotUserListItemText);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.Message);
            }
        }

        private void ShowPositionsListClickHandler(object sender, RoutedEventArgs e)
        {
            var rolesWindow = new ListOfRolesWindow();
            rolesWindow.Show();
            Close();
        }
    }
}

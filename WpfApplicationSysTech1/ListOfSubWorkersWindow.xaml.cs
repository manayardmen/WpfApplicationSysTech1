using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using WpfApplicationSysTech1.app;
using WpfApplicationSysTech1.app.models;
using WpfApplicationSysTech1.app.repo;
using WpfApplicationSysTech1.app.types;

namespace WpfApplicationSysTech1
{
    public partial class ListOfSubWorkersWindow
    {
        private readonly AppMain _appMain;

        private UserListItem _selectedSubUser;

        public ListOfSubWorkersWindow()
        {
            InitializeComponent();

            _appMain = AppMain.Instance;

            if (!_appMain.IsAdmin && !_appMain.IsModer)
            {
                AddNewSubWorker.Visibility = Visibility.Hidden;
                RemoveSubWorker.Visibility = Visibility.Hidden;
            }

            ShowBlockControlsBorder();
            new Thread(GetSubWorkersList).Start();
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

        private void GetSubWorkersList()
        {
            var usersList = UsersRepo.GetSubWorkersUsers(SelectedUserData.SelectedUser.Id);
            Dispatcher.Invoke(() =>
            {
                UsersListView.ItemsSource = DataConverter.GetUsersList(usersList, _appMain.Positions);
                ResetBlockControlsState();
            });
        }

        private void GoBackToUserProfileClickHandler(object sender, RoutedEventArgs e)
        {
            if (SelectedUserData.SelectedUser.Id == AppMain.Instance.CurrentUser.Id)
            {
                SelectedUserData.ResetSelectedUser();
                SelectedUserData.ResetSubSelectedUser();

                // Мой профиль
                var myUserWindow = new MyProfileWindow();
                myUserWindow.Show();
                Close();
            }
            else
            {
                var userWindow = new UserProfileWindow();
                userWindow.Show();
                Close();
            }
        }

        private void UsersListSelectionChangeHandler(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var item = (sender as ListView)?.SelectedItem;
                if (item is UserListItem user)
                {
                    Console.WriteLine(user.Name);
                    Console.WriteLine(user.Login);

                    _selectedSubUser = user;
                    SelectedUserTextBlock.Text = $"Выбранный сотрудник: {_selectedSubUser.Name}, {_selectedSubUser.Login}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show(ex.Message, Const.AppName);
            }
        }

        private void AddNewSubWorkerClickHandler(object sender, RoutedEventArgs e)
        {
            var usersWindow = new AddNewSubWorkerWindow();
            usersWindow.Show();
            Close();
        }

        private void RemoveSubWorkerClickHandler(object sender, RoutedEventArgs e)
        {
            if (_selectedSubUser != null)
            {
                ShowBlockControlsBorder();
                new Thread(RemoveBossFromThisUser).Start();
            }
            else
            {
                Console.WriteLine("Ошибка не выбран пользователь на удаление");
                MessageBox.Show("Ошибка, не выбран пользователь", Const.AppName);
            }
        }

        private void RemoveBossFromThisUser()
        {
            UsersRepo.ResetToThisUserThisBoss(_selectedSubUser.Id);
            GetSubWorkersList();

            Dispatcher.Invoke(() =>
            {
                _selectedSubUser = null;
                SelectedUserTextBlock.Text = "Выбранный сотрудник: [нет выбранного пользователя]";
                //ResetBlockControlsState();
            });
        }
    }
}

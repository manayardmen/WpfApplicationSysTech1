using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using WpfApplicationSysTech1.app;
using WpfApplicationSysTech1.app.repo;
using WpfApplicationSysTech1.app.types;

namespace WpfApplicationSysTech1
{
    public partial class AddNewSubWorkerWindow
    {
        public AddNewSubWorkerWindow()
        {
            InitializeComponent();

            ShowBlockControlsBorder();
            new Thread(LoadUsers).Start();
        }

        private void LoadUsers()
        {
            var users = UsersRepo.GetFreeWorkersUsers(SelectedUserData.SelectedUser.Id, SelectedUserData.SelectedUser.BossId);

            Dispatcher.Invoke(() =>
            {
                //UsersListView.ItemsSource = users;
                UsersListView.ItemsSource = DataConverter.GetUsersList(users, AppMain.Instance.Positions);
                ResetBlockControlsState();
            });
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

        private void UsersListSelectionChangeHandler(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var item = (sender as ListView)?.SelectedItem;
                if (item is UserListItem user)
                {
                    Console.WriteLine(user.Name);
                    Console.WriteLine(user.Login);

                    ShowBlockControlsBorder();
                    new Thread(() =>
                    {
                        SetBossToUser(user);
                    }).Start();
                }
                else
                {
                    Console.WriteLine("Элемент не имеет нужного типа -> UserListItem");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show(ex.Message, Const.AppName);
            }
        }

        private void SetBossToUser(UserListItem user)
        {
            UsersRepo.SetToThisUserThisBoss(user.Id, SelectedUserData.SelectedUser.Id);
            Dispatcher.Invoke(
                () =>
                {
                    ResetBlockControlsState();

                    var usersWindow = new ListOfSubWorkersWindow();
                    usersWindow.Show();
                    Close();
                });
        }

        private void CancelButtonClickHandler(object sender, RoutedEventArgs e)
        {
            var usersWindow = new ListOfSubWorkersWindow();
            usersWindow.Show();
            Close();
        }
    }
}

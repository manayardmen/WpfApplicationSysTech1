using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using WpfApplicationSysTech1.app;
using WpfApplicationSysTech1.app.models;
using WpfApplicationSysTech1.app.repo;

namespace WpfApplicationSysTech1
{
    public partial class SelectNewRoleForUserWindow
    {
        public SelectNewRoleForUserWindow()
        {
            InitializeComponent();

            SelectPositionToUserTextTitle.Text = $"Выбор должности для {SelectedUserData.SelectedUser.Name}";

            //ShowBlockControlsBorder();
            //new Thread(LoadPositionsList).Start();
            LoadPositionsList();
        }

        private void LoadPositionsList()
        {
            PositionsListView.ItemsSource = AppMain.Instance.Positions;
            //ResetBlockControlsState();
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

        // Выбор позиции для текущего выбранного пользователя из списка
        private void PositionsListSelectionChangeHandler(object sender, SelectionChangedEventArgs e)
        {
            // Устанавливаем пользователю новую роль
            try
            {
                var item = (sender as ListView)?.SelectedItem;
                if (item is Position position)
                {
                    Console.WriteLine(position.Name);

                    var userId = SelectedUserData.SelectedUser.Id;
                    var positionId = position.Id;

                    ShowBlockControlsBorder();

                    new Thread(() =>
                    {
                        SetNewRoleToUser(userId, position);
                    }).Start();
                }
            }
            catch (Exception ex)
            {
                ResetBlockControlsState();
                Console.WriteLine(ex.Message);
            }
        }

        // Установка новой роли пользователю
        private void SetNewRoleToUser(int userId, Position position)
        {
            UsersRepo.ChangeUserRole(userId, position.Id);

            SelectedUserData.SelectedUser.PositionId = position.Id;

            SelectedUserData.SelectedUserListItem.PositionId = position.Id;
            SelectedUserData.SelectedUserListItem.PositionName = position.Name;

            Dispatcher.Invoke(() =>
            {
                ResetBlockControlsState();
                GoToUserProfile();
            });
        }

        // Переход назад к профилю пользователя
        private void GoToUserProfile()
        {
            var userWindow = new UserProfileWindow();
            userWindow.Show();
            Close();
        }

        // Возвращаемся в профиль пользователя
        private void CancelButtonClickHandler(object sender, RoutedEventArgs e) => GoToUserProfile();
    }
}

﻿using System.Threading;
using System.Windows;
using System.Windows.Media;
using WpfApplicationSysTech1.app;
using WpfApplicationSysTech1.app.repo;

namespace WpfApplicationSysTech1
{
    public partial class AddNewUserWindow
    {
        public AddNewUserWindow()
        {
            InitializeComponent();
        }

        private void BackToUsersList()
        {
            var usersWindow = new UsersListWindow();
            usersWindow.Show();
            Close();
        }

        private void BackToUsersListButtonClickHandler(object sender, RoutedEventArgs e) => BackToUsersList();

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

        private void AddNewUser(string name, string login, string password)
        {
            var addUserIsSuccess = UsersRepo.AddNewUser(name, login, password);

            MessageBox.Show(
                addUserIsSuccess ? "Новый пользователь успешно добавлен" : "Не удалось добавить нового пользователя",
                Const.AppName);

            Dispatcher.Invoke(ResetBlockControlsState);
        }

        private void AddNewUserClickHandler(object sender, RoutedEventArgs e)
        {
            string name = TextBoxName.Text.Trim();
            string login = TextBoxLogin.Text.Trim();
            string password = PasswordBox.Password.Trim();
            string passwordRepeat = PasswordBoxRepeat.Password.Trim();

            bool isNoErrorsFound = true;

            if (name.Length < Const.MinFieldLength)
            {
                TextBoxName.ToolTip = Const.ErrorTooltip;
                TextBoxName.Background = Brushes.Crimson;
                isNoErrorsFound = false;
            }
            else
            {
                TextBoxName.ToolTip = string.Empty;
                TextBoxName.Background = Brushes.Transparent;
            }

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

            if (password != passwordRepeat || string.IsNullOrEmpty(passwordRepeat))
            {
                PasswordBoxRepeat.ToolTip = Const.ErrorTooltip;
                PasswordBoxRepeat.Background = Brushes.Crimson;
                isNoErrorsFound = false;
            }
            else
            {
                PasswordBoxRepeat.ToolTip = string.Empty;
                PasswordBoxRepeat.Background = Brushes.Transparent;
            }

            if (isNoErrorsFound)
            {
                ShowBlockControlsBorder();
                new Thread(() => AddNewUser(name, login, password)).Start();
            }
        }
    }
}

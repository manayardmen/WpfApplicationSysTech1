using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using WpfApplicationSysTech1.app;

namespace WpfApplicationSysTech1
{
    public partial class SalaryOnDateWindow
    {
        private DateTime _selectedDate = DateTime.Now;

        public SalaryOnDateWindow()
        {
            InitializeComponent();

            WorkerTextBlock.Text = "Для сотрудника: " + SelectedUserData.SelectedUser.Name;

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

        private void BackClickHandler(object sender, RoutedEventArgs e)
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

        private void OnSelectedDateChangedHandler(object sender, SelectionChangedEventArgs e)
        {
            var selectedDate = SalaryDatePicker.SelectedDate;
            if (selectedDate != null)
            {
                _selectedDate = (DateTime) selectedDate;
            }
            else
            {
                Console.WriteLine("Ошибка, нет выбранной даты");
            }
        }

        private void CalcSalaryClickHandler(object sender, RoutedEventArgs e)
        {
            ShowBlockControlsBorder();
            new Thread(CalcSalary).Start();
        }

        private void CalcSalary()
        {
            var salaryResult = SalaryWorker.GetSalaryForThisWorker(SelectedUserData.SelectedUser.Id,
                SelectedUserData.SelectedUserListItem.IncomeDate, _selectedDate);

            //SalaryResultTextBlock
            //Результат:

            Dispatcher.Invoke(() =>
            {
                SalaryResultTextBlock.Text = "Результат: " + salaryResult;
                ResetBlockControlsState();
            });
        }
    }
}

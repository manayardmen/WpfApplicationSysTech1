using System.Windows;
using WpfApplicationSysTech1.app;

namespace WpfApplicationSysTech1
{
    public partial class ListOfRolesWindow
    {
        public ListOfRolesWindow()
        {
            InitializeComponent();
            //ShowBlockControlsBorder();
            //new Thread(LoadPositionsList).Start();
            LoadPositionsList();
        }

        private void LoadPositionsList()
        {
            PositionsListView.ItemsSource = AppMain.Instance.Positions;
            //ResetBlockControlsState();
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

        private void GoToUsersListClickHandler(object sender, RoutedEventArgs e)
        {
            var usersWindow = new UsersListWindow();
            usersWindow.Show();
            Close();
        }
    }
}

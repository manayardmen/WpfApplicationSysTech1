using WpfApplicationSysTech1.app.models;
using WpfApplicationSysTech1.app.types;

namespace WpfApplicationSysTech1.app
{
    // Инструменты для работы с выбранным из списка пользователем
    public static class SelectedUserData
    {
        // Выбранный пользователь
        public static User SelectedUser { get; private set; }
        public static UserListItem SelectedUserListItem { get; private set; }

        // Выбранный пользователь (из списка подчиненных)
        public static User SubSelectedUser { get; private set; }
        public static UserListItem SubSelectedUserListItem { get; private set; }

        public static void SetSelectedUser(User u1 = null, UserListItem u2 = null)
        {
            SelectedUser = u1;
            SelectedUserListItem = u2;
        }

        public static void ResetSelectedUser()
        {
            SetSelectedUser();
        }

        public static void SetSubSelectedUser(User u1 = null, UserListItem u2 = null)
        {
            SubSelectedUser = u1;
            SubSelectedUserListItem = u2;
        }

        public static void ResetSubSelectedUser()
        {
            SetSubSelectedUser(null, null);
        }
    }
}

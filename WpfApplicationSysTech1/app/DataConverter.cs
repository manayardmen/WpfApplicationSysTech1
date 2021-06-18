using System.Collections.Generic;
using WpfApplicationSysTech1.app.models;
using WpfApplicationSysTech1.app.types;

namespace WpfApplicationSysTech1.app
{
    public static class DataConverter
    {
        public static List<UserListItem> GetUsersList(List<User> users, List<Position> positions)
        {
            var resultList = new List<UserListItem>();

            foreach (var u in users)
                resultList.Add(new UserListItem(u, positions));

            return resultList;
        }

        public static List<PositionListItem> GetPositionsList(List<Position> positions)
        {
            var resultList = new List<PositionListItem>();

            foreach (var p in positions)
                resultList.Add(new PositionListItem(p));

            return resultList;
        }
    }
}

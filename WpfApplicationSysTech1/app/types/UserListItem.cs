using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WpfApplicationSysTech1.app.models;
using WpfApplicationSysTech1.Properties;

namespace WpfApplicationSysTech1.app.types
{
    public class UserListItem
    {
        public int Id { get; set; }                 // Идентификатор пользователя
        public string Name { get; set; }            // Имя пользователя (сотрудника)
        public string Login { get; set; }           // Логин
        public string Password { get; set; }        // Пароль
        public DateTime IncomeDate { get; set; }    // Дата поступления на работу
        public string IncomeDateStr { get; set; }    // Дата поступления на работу
        public int PositionId { get; set; }         // Идентификатор должности
        public bool IsActive { get; set; }          // Активен ли пользователь
        public int BossId { get; set; }             // Идентификатор начальника
        public string PositionName { get; set; }    // Название позиции
        public UserListItem(User u, List<Position> p)
        {
            Id = u.Id;
            Name = u.Name;
            Login = u.Login;
            Password = u.Password;
            IncomeDateStr = u.IncomeDate;

            try
            {
                //IncomeDate = Convert.ToDateTime(u.IncomeDate);

                CultureInfo provider = CultureInfo.InvariantCulture;
                IncomeDate = DateTime.ParseExact(u.IncomeDate, "MM.dd.yyyy", provider);
            }
            catch (Exception e)
            {
                IncomeDate = new DateTime();
                Console.WriteLine(e.Message);
            }

            PositionId = u.PositionId;
            IsActive = u.IsActive == 1;
            BossId = u.BossId;

            if (p != null)
            {
                var pos = p.FirstOrDefault(x => x.Id == u.PositionId);
                if (pos != null) PositionName = pos.Name;
            }
            else
            {
                Console.WriteLine(Resources.UserListItemCreationErrorText);
            }
        }
    }
}

namespace WpfApplicationSysTech1.app.models
{
    // Модель данных для таблицы Users
    public class User
    {
        public int Id { get; set; }             // Идентификатор пользователя
        public string Name { get; set; }        // Имя пользователя (сотрудника)
        public string Login { get; set; }       // Логин
        public string Password { get; set; }    // Пароль
        public string IncomeDate { get; set; }  // Дата поступления на работу
        public int PositionId { get; set; }     // Идентификатор должности
        public int IsActive { get; set; }       // Активен ли пользователь
        public int BossId { get; set; }         // Идентификатор начальника

        public User() { }

        public User(
            string Name,
            string Login,
            string Password,
            string IncomeDate,
            int PositionId,
            int IsActive,
            int BossId)
        {
            this.Name = Name;
            this.Login = Login;
            this.Password = Password;
            this.IncomeDate = IncomeDate;
            this.PositionId = PositionId;
            this.IsActive = IsActive;
            this.BossId = BossId;
        }
    }
}

using System.Data.Entity;
using WpfApplicationSysTech1.app.models;

namespace WpfApplicationSysTech1.app.context
{
    // Контекст данных
    public class ApplicationContext: DbContext
    {
        // Таблицы из БД
        public DbSet<User> Users { get; set; }
        public DbSet<Position> Positions { get; set; }

        public ApplicationContext(): base("DefaultConnection") { }
    }
}

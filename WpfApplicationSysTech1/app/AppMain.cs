using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using WpfApplicationSysTech1.app.context;
using WpfApplicationSysTech1.app.models;
using WpfApplicationSysTech1.app.repo;
using WpfApplicationSysTech1.app.settings;
using WpfApplicationSysTech1.Properties;

namespace WpfApplicationSysTech1.app
{
    // Основной класс приложения
    public class AppMain
    {
        // Win32 base APIs
#if DEBUG
        [DllImport("Kernel32")]
        private static extern void AllocConsole();
#endif

        private static AppMain _instance;

        // Статический экземпляр объекта
        public static AppMain Instance => _instance ?? (_instance = new AppMain());

        public bool IsAdmin { get; private set; }
        public bool IsModer { get; private set; }
        public AppSettings AppSettings { get; private set; }
        private EventsHub EventsHub { get; }
        public ApplicationContext Context { get; }

        public User CurrentUser { get; private set; }
        public List<Position> Positions { get; private set; }

        private AppMain()
        {
#if DEBUG
            AllocConsole();
#endif

            Context = new ApplicationContext();
            EventsHub = new EventsHub();
            AppSettings = new AppSettings();

            LoadAppSettings();
            CheckDbFileExisting();
        }

        // Загрузить массив позиций
        public void LoadPositionsArr()
        {
            Console.WriteLine(Resources.LoadingPositionsArrText);
            Positions = Context.Positions.ToList();
            Console.WriteLine(Positions.Count);
        }

        public void ResetCurrentUser()
        {
            IsAdmin = false;
            IsModer = false;

            CurrentUser = null;
        }

        // Мы зашли как администратор (установить текущего пользователя)
        public void SetLoginInAppAsAdmin()
        {
            CurrentUser = new User
            {
                Name = Const.AdminName,
                Login = AppSettings.AdminLogin
            };

            IsAdmin = true;
            IsModer = false;
        }

        // Мы зашли как определенный пользователь
        public void SetLoginInAppAsUser(User user)
        {
            CurrentUser = user;

            // Мы точно не администратор
            IsAdmin = false;

            // Но можем обладать ролью редактора (модератора)
            var editorPositionId = PositionsRepo.GetEditorPositionId();
            IsModer = user.PositionId == editorPositionId;
        }

        private void CheckDbFileExisting()
        {
            if (!File.Exists(Const.DefaultDbFilePath))
            {
                MessageBox.Show(Resources.DbFileNotFoundText, Const.AppName);
                Console.WriteLine(Resources.DbFileNotFoundText);
            }
        }

        // Загрузить настройки приложения
        private void LoadAppSettings()
        {
            if (File.Exists(Const.AppSettingsFilePath))
            {
                // Считать настройки из файла
                var input = File.ReadAllText(Const.AppSettingsFilePath);
                AppSettings = JsonSerializer.Deserialize<AppSettings>(input);
            }
            else
            {
                MessageBox.Show(Resources.AppSettigsFileNotFoundText, Const.AppName);
                Console.WriteLine(Resources.AppSettigsFileNotFoundText);
            }

            EventsHub.InvokeAppSettingLoadedEvent();
        }
    }
}

namespace WpfApplicationSysTech1.app.settings
{
    // Настройки приложения
    public class AppSettings
    {
        public string AdminLogin { get; set; } = Const.DefaultAdminLogin;
        public string AdminPassword { get; set; } = Const.DefaultAdminPassword;
        public string BasicPositionName { get; set; } = Const.DefaultBasicPositionName;
        public string EditorPositionName { get; set; } = Const.DefaultEditorPositionName;
    }
}

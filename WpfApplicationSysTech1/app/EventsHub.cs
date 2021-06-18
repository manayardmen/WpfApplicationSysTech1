namespace WpfApplicationSysTech1.app
{
    // Хаб событий приложения
    public class EventsHub
    {
        // Тип обработчика для событий
        public delegate void Handler();

        // Событие завершения загрузки настроек приложения
        public event Handler AppSettingLoadedEvent;
        // Генерация события завершения загрузки настроек приложения
        public void InvokeAppSettingLoadedEvent() => AppSettingLoadedEvent?.Invoke();
    }
}

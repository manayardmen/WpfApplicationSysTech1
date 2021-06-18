namespace WpfApplicationSysTech1.app.models
{
    // Модель данных для таблицы Positions
    public class Position
    {
        public int Id { get; set; }                 // Идентификатор позиции (должности)
        public string Name { get; set; }            // Название позиции
        public int IsCanHaveSubs { get; set; }      // Может ли сотрудник этой должности иметь подчиненных
        public int BaseRate { get; set; }           // Базовая ставка
        public int PerYearRate { get; set; }        // Процентная надбавка за год работы
        public int MaxRate { get; set; }            // Максимальная процентная надбавка
        public double SubsRate { get; set; }        // Надбавка от зарплаты всех подчиненныъ
        public int IsRateOfAllLayers { get; set; }  // Учитывает ли надбавка подчиненных всех уровней

        public Position() { }

        public Position(
            string Name,
            int IsCanHaveSubs,
            int BaseRate,
            int PerYearRate,
            int MaxRate,
            int SubsRate,
            int IsRateOfAllLayers)
        {
            this.Name = Name;
            this.IsCanHaveSubs = IsCanHaveSubs;
            this.BaseRate = BaseRate;
            this.PerYearRate = PerYearRate;
            this.MaxRate = MaxRate;
            this.SubsRate = SubsRate;
            this.IsRateOfAllLayers = IsRateOfAllLayers;
        }
    }
}

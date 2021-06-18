using WpfApplicationSysTech1.app.models;

namespace WpfApplicationSysTech1.app.types
{
    public class PositionListItem
    {
        public int Id { get; set; }                     // Идентификатор позиции (должности)
        public string Name { get; set; }                // Название позиции
        public bool IsCanHaveSubs { get; set; }         // Может ли сотрудник этой должности иметь подчиненных
        public int BaseRate { get; set; }               // Базовая ставка
        public int PerYearRate { get; set; }            // Процентная надбавка за год работы
        public int MaxRate { get; set; }                // Максимальная процентная надбавка
        public double SubsRate { get; set; }            // Надбавка от зарплаты всех подчиненныъ
        public bool IsRateOfAllLayers { get; set; }     // Учитывает ли надбавка подчиненных всех уровней
        public PositionListItem(Position p)
        {
            Id = p.Id;
            Name = p.Name;
            IsCanHaveSubs = p.IsCanHaveSubs == 1;
            BaseRate = p.BaseRate;
            PerYearRate = p.PerYearRate;
            MaxRate = p.MaxRate;
            SubsRate = p.SubsRate;
            IsRateOfAllLayers = p.IsRateOfAllLayers == 1;
        }
    }
}

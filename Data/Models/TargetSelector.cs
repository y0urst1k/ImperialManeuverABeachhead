using Data.Enums;

namespace Data.Models
{
    public class TargetSelector
    {
        public ZoneType Zone { get; set; }
        public OwnerType Owner { get; set; }
        public UnitTypeFilter UnitType { get; set; }
        public int Count { get; set; }
        public SelectionMode Mode { get; set; }
    }
}

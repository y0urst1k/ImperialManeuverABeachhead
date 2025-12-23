using Data.Enums;

namespace Data.Models
{
    public class AbilityModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public AbilityTrigger Trigger { get; set; }
        public int ResourceCost { get; set; } // Только для Active

        // Параметры для движка
        public EffectType Effect { get; set; }
        public int Value { get; set; } // Сила эффекта (например, 2 урона или +1 к атаке)
        public TargetType Target { get; set; } // Enemies, Allies, Self, FrontlineOnly
    }
}
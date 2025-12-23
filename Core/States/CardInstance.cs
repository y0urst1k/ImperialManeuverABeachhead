using Data.Models;

namespace Core.States
{
    // Экземпляр карты на поле (отличается от CardModel!)
    public class CardInstance
    {
        public Guid InstanceId { get; } = Guid.NewGuid(); // Уникальный ID конкретной карты на столе
        public CardModel BaseData { get; set; } // Ссылка на "шаблон" карты (неизменяемые данные)

        // Текущие характеристики (могут меняться: баффы, ранения)
        public int CurrentHealth { get; set; }
        public int CurrentAttack { get; set; }
        public bool IsExhausted { get; set; } // Усталость

        // Временные модификаторы (ауры)
        public int AttackModifier { get; set; }
    }
}

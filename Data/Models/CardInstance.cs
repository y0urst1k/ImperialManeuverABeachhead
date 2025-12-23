using Data.Models;

namespace Core.States
{
    // Экземпляр карты на поле (отличается от CardModel!)
    public class CardInstance
    {
        public Guid InstanceId { get; } = Guid.NewGuid();
        public Guid OwnerId { get; set; } // Чья это карта (Player или Enemy)
        public CardModel BaseData { get; set; } // Ссылка на неизменяемые данные

        public int CurrentHealth { get; set; }
        public int CurrentAttack { get; set; }
        public bool IsExhausted { get; set; } // Усталость (не может атаковать)

        // Для аур (баффы, которые пропадают, если аура исчезла)
        public int BuffAttack { get; set; }
        public int BuffHealth { get; set; }

        // Итоговая атака (Base + Buff)
        public int TotalAttack => CurrentAttack + BuffAttack;
    }
}

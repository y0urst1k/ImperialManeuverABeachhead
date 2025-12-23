using Data.Models;

namespace MainComponents.ViewModels
{
    public class CardInstance
    {
        public Guid Id { get; set; } // Уникальный ID в рамках матча
        public CardModel BaseData { get; set; } // Ссылка на неизменяемые данные
        public int CurrentHealth { get; set; }
        public int CurrentAttack { get; set; }
        public bool IsExhausted { get; set; }
    }
}

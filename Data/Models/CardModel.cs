using Data.Enums;

namespace Data.Models
{
    public class CardModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } // Историческая справка
        public string ImagePath { get; set; }

        public int Cost { get; set; }      // Стоимость разыгрывания
        public int Attack { get; set; }    // Атака
        public int Health { get; set; }    // Здоровье
        public int Production { get; set; } // Сколько ресурсов дает (для тыловых карт)

        public CardType Type { get; set; }

        // Свойства для игры (изменяемые)
        public bool IsExhausted { get; set; } // Ходил ли в этом ходу

        public List<AbilityModel> Abilities { get; set; } = new();
    }
}
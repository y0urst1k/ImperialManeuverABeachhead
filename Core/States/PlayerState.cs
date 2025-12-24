namespace Core.States
{
    // Состояние конкретного игрока в матче
    public class PlayerState
    {
        public Guid PlayerId { get; set; } // Ссылка на профиль (Имя, Аватар)

        // Динамические параметры матча
        public int CurrentHealth { get; set; }
        public int CurrentResources { get; set; }

        // Зоны
        public List<CardInstance> Hand { get; set; } = new();
        public List<CardInstance> Deck { get; set; } = new(); // Библиотека (колода в игре)
        public List<CardInstance> Graveyard { get; set; } = new(); // Сброс

        public List<CardInstance> Frontline { get; set; } = new(); // Стол (Фронт)
        public List<CardInstance> Backline { get; set; } = new();  // Стол (Тыл)
    }
}

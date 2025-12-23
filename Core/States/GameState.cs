namespace Core.States
{
    public class GameState
    {
        public Guid MatchId { get; } = Guid.NewGuid();
        public int TurnNumber { get; set; } = 1;

        // Чей сейчас ход? (true = Игрок 1, false = Игрок 2/AI)
        public bool IsPlayerTurn { get; set; } = true;

        public PlayerState Player { get; set; }
        public PlayerState Enemy { get; set; }
    }
}

using Core.States;
using Data.Enums;
using Data.Models;

namespace Core.Interface
{
    public interface IGameEngineService
    {
        GameState State { get; }
        event EventHandler StateChanged;

        void StartGame(DeckModel playerDeck, DeckModel enemyDeck);
        void PlayCard(Guid cardInstanceId, TargetType targetZone); // Розыгрыш с руки
        void EndTurn();
        void Attack(Guid attackerId, Guid targetId);
    }
}

using Core.Interface;
using Core.States;
using Data.Enums;
using Data.Models;
using Services.Interface;

namespace Core.Service
{
    public class GameEngineService : IGameEngineService
    {
        public GameState State { get; private set; }
        public event EventHandler StateChanged;

        private readonly ICardDatabaseService _cardDb; // Сервис для получения CardModel по ID

        public GameEngineService(ICardDatabaseService cardDb)
        {
            _cardDb = cardDb;
        }

        public void StartGame(DeckModel playerDeck, DeckModel enemyDeck)
        {
            State = new GameState
            {
                Player = InitPlayer(playerDeck),
                Enemy = InitPlayer(enemyDeck)
            };

            DrawCards(State.Player, 3); // Стартовая рука
            DrawCards(State.Enemy, 3);

            Notify();
        }

        private PlayerState InitPlayer(DeckModel deck)
        {
            var state = new PlayerState
            {
                Profile = new PlayerModel { Name = "Player", Health = 30, CurrentResources = 1 },
                CurrentHealth = 30,
                CurrentResources = 1
            };

            // Превращаем ID карт из колоды в CardInstance
            foreach (var cardId in deck.CardIds)
            {
                var cardModel = _cardDb.GetCardById(cardId);
                var instance = new CardInstance
                {
                    BaseData = cardModel,
                    CurrentHealth = cardModel.Health,
                    CurrentAttack = cardModel.Attack
                };
                state.Deck.Add(instance);
            }

            // Перемешиваем (Shuffle) - можно добавить метод
            return state;
        }

        public void PlayCard(Guid cardInstanceId, TargetType targetZone)
        {
            var player = State.Player;
            var card = player.Hand.FirstOrDefault(c => c.InstanceId == cardInstanceId);

            if (card == null) return; // Ошибка: карты нет в руке
            if (player.CurrentResources < card.BaseData.Cost) return; // Ошибка: мало маны

            // 1. Тратим ресурсы
            player.CurrentResources -= card.BaseData.Cost;
            player.Hand.Remove(card);

            // 2. Кладем на стол
            if (targetZone == TargetType.Frontline)
                player.Frontline.Add(card);
            else if (targetZone == TargetType.Backline)
                player.Backline.Add(card);

            // 3. Срабатывание "OnPlay" способностей
            TriggerAbilities(card, AbilityTrigger.OnPlay);

            Notify();
        }

        public void EndTurn()
        {
            // Смена хода
            State.IsPlayerTurn = !State.IsPlayerTurn;

            var activePlayer = State.IsPlayerTurn ? State.Player : State.Enemy;

            // Прирост ресурсов (База + Производство)
            int income = 1; // База
            foreach (var card in activePlayer.Backline)
            {
                income += card.BaseData.Production;
            }
            activePlayer.CurrentResources += income;

            // Добор карты
            DrawCards(activePlayer, 1);

            // Сброс усталости
            foreach (var card in activePlayer.Frontline) card.IsExhausted = false;

            Notify();
        }

        private void TriggerAbilities(CardInstance source, AbilityTrigger trigger)
        {
            foreach (var ability in source.BaseData.Abilities)
            {
                if (ability.Trigger == trigger)
                {
                    // Тут логика применения эффекта (AbilityProcessor)
                    // ApplyEffect(ability, source);
                }
            }
        }

        private void DrawCards(PlayerState player, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (player.Deck.Count > 0)
                {
                    var card = player.Deck[0];
                    player.Deck.RemoveAt(0);
                    player.Hand.Add(card);
                }
            }
        }

        private void Notify() => StateChanged?.Invoke(this, EventArgs.Empty);

        // Заглушка для атаки
        public void Attack(Guid attackerId, Guid targetId) { /* ... */ Notify(); }
    }
}

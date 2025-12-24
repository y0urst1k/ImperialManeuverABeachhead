using Core.Interface;
using Core.States;
using Data.Enums;
using Data.Models;
using Services.Interface;

namespace Core.Service
{
    public class GameEngineService : IGameEngineService
    {
        private readonly ISessionContextService _sessionContext;
        private readonly AbilityProcessor _abilityProcessor;
        private readonly ICardDatabaseService _cardDb; // Сервис для получения CardModel по ID

        public GameState State { get; private set; }
        public event EventHandler StateChanged;

        public GameEngineService(ICardDatabaseService cardDb, AbilityProcessor abilityProcessor, ISessionContextService sessionContext)
        {
            _cardDb = cardDb;
            _abilityProcessor = abilityProcessor;
            _sessionContext = sessionContext;
        }

        public void StartGame(DeckModel enemyDeck)
        {
            var playerDeck = _sessionContext.ActiveDeck;

            if (playerDeck == null)
                throw new Exception("Active deck not selected!");

            State = new GameState
            {
                Player = InitPlayer(playerDeck, isHuman: true),
                Enemy = InitPlayer(enemyDeck, isHuman: false)
            };

            DrawCards(State.Player, 3); // Стартовая рука
            DrawCards(State.Enemy, 3);

            Notify();
        }

        private PlayerState InitPlayer(DeckModel deck, bool isHuman)
        {
            // Если это человек, берем профиль из контекста. Если бот - создаем фейковый.
            var profile = isHuman
                ? _sessionContext.CurrentPlayer
                : new PlayerModel { Id = Guid.NewGuid(), Name = "Bot", Health = 30 };

            var state = new PlayerState
            {
                PlayerId = profile.Id,
                CurrentHealth = 30,
                CurrentResources = 0 // Начинаем с 0
            };

            // Превращаем ID карт из колоды в CardInstance
            foreach (var cardId in deck.CardIds)
            {
                var cardModel = _cardDb.GetCardById(cardId);
                var instance = new CardInstance
                {
                    BaseData = cardModel,
                    CurrentHealth = cardModel.Health,
                    CurrentAttack = cardModel.Attack,
                    OwnerId = profile.Id
                };
                state.Deck.Add(instance);
            }

            // Перемешиваем (Shuffle) - можно добавить метод
            return state;
        }

        public void PlayCard(Guid cardInstanceId, ZoneType targetZone)
        {
            var player = State.Player;
            var card = player.Hand.FirstOrDefault(c => c.InstanceId == cardInstanceId);

            if (card == null) return; // Ошибка: карты нет в руке
            if (player.CurrentResources < card.BaseData.Cost) return; // Ошибка: мало маны

            // 1. Тратим ресурсы
            player.CurrentResources -= card.BaseData.Cost;
            player.Hand.Remove(card);

            // 2. Кладем на стол
            if (targetZone == ZoneType.Frontline)
                player.Frontline.Add(card);
            else if (targetZone == ZoneType.Backline)
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
                    // Передаем ТЕКУЩЕЕ состояние игры
                    _abilityProcessor.Process(State, ability, source, _cardDb);
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
        public void Attack(Guid attackerId, Guid targetId) 
        {
            var attacker = FindCardOnBoard(attackerId);
            var target = FindCardOnBoard(targetId);

            if (attacker == null || target == null) return;
            if (attacker.IsExhausted) return; // Не может атаковать
            if (attacker.CurrentAttack <= 0) return;

            // 1. Бой (Взаимный урон)
            target.CurrentHealth -= attacker.TotalAttack;
            attacker.CurrentHealth -= target.TotalAttack;

            // 2. Усталость
            attacker.IsExhausted = true;

            // 3. Проверка смертей (Вот тут она нужна!)
            ResolveDeaths();
            Notify(); 
        }

        private CardInstance FindCardOnBoard(Guid instanceId)
        {
            // 1. Ищем у Игрока (Player)
            var card = FindInPlayerZones(State.Player, instanceId);
            if (card != null) return card;

            // 2. Ищем у Врага (Enemy)
            return FindInPlayerZones(State.Enemy, instanceId);
        }

        private CardInstance FindInPlayerZones(PlayerState player, Guid instanceId)
        {
            // Проверяем Фронт
            var inFront = player.Frontline.FirstOrDefault(c => c.InstanceId == instanceId);
            if (inFront != null) return inFront;

            // Проверяем Тыл
            var inBack = player.Backline.FirstOrDefault(c => c.InstanceId == instanceId);
            return inBack;
        }

        // Метод "Уборщик"
        private void ResolveDeaths()
        {
            // Проходим по Frontline и Backline обоих игроков
            // Удаляем тех, у кого Health <= 0
            // Добавляем их в Graveyard
            // Вызываем Deathrattle (если будут)
        }
    }
}

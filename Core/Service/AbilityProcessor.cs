using Core.States;
using Data.Enums;
using Data.Models;
using Services.Interface;

namespace Core.Service
{
    public class AbilityProcessor
    {

        public void Process(GameState state, AbilityModel ability, CardInstance source, ICardDatabaseService _cardDb)
        {
            if (!CheckCondition(state, ability, source))
                return;

            switch (ability.Effect)
            {
                case EffectType.Damage:
                    ApplyDamage(state, ability, source);
                    break;

                case EffectType.BuffAttack:
                    source.CurrentAttack += ability.Value;
                    break;

                case EffectType.GenerateResource:
                    if (state.IsPlayerTurn) 
                        state.Player.CurrentResources += ability.Value;
                    else 
                        state.Enemy.CurrentResources += ability.Value;
                    break;
                case EffectType.Summon:
                    // Value = ID карты, которую надо призвать (или ConditionParam использовать для ID)
                    // Предполагаем, что у нас есть доступ к CardDatabaseService (надо внедрить его)
                    var tokenCard = _cardDb.GetCardById(Guid.Parse(ability.ConditionParam));

                    var tokenInstance = new CardInstance
                    {
                        BaseData = tokenCard,
                        CurrentHealth = tokenCard.Health,
                        
                    };

                    var owner = GetOwnerState(state, source);
                    owner.Frontline.Add(tokenInstance);
                    break;
            }
        }

        private void ApplyDamage(GameState state, AbilityModel ability, CardInstance source)
        {
            // Логика выбора цели
            var enemyState = (GetOwnerState(state, source) == state.Player) ? state.Enemy : state.Player;

            var target = enemyState.Frontline.FirstOrDefault();
            if (target != null)
            {
                target.CurrentHealth -= ability.Value;
                CheckDeath(state, target);
            }
        }

        // Метод выбора целей (Очень важная часть!)
        private List<CardInstance> GetTargets(GameState state, TargetType targetType, CardInstance source)
        {
            var targets = new List<CardInstance>();
            var enemyState = (source.OwnerId == state.Player.Profile.Id) ? state.Enemy : state.Player;
            var friendlyState = (source.OwnerId == state.Player.Profile.Id) ? state.Player : state.Enemy;

            switch (targetType)
            {
                case TargetType.Enemy: // Пример нового типа цели
                    targets.AddRange(enemyState.Frontline);
                    break;
                case TargetType.Self:
                    targets.Add(source);
                    break;
                    // ...
            }
            return targets;
        }

        private void CheckDeath(GameState state, CardInstance unit)
        {
            // Удаляем из списков (ищем и у игрока, и у врага)
            if (state.Player.Frontline.Remove(unit) || state.Player.Backline.Remove(unit))
            {
                state.Player.Graveyard.Add(unit);
            }
            else if (state.Enemy.Frontline.Remove(unit) || state.Enemy.Backline.Remove(unit))
            {
                state.Enemy.Graveyard.Add(unit);
            }
        }

        private bool CheckCondition(GameState state, AbilityModel ability, CardInstance source)
        {
            switch (ability.Condition)
            {
                case ConditionType.None:
                    return true;

                case ConditionType.IfFrontlineHasUnit:
                    var owner = GetOwnerState(state, source);
                    // Проверка: есть ли на фронте карта с таким именем/типом
                    return owner.Frontline.Any(c => c.BaseData.Name == ability.ConditionParam);

                default:
                    return false;
            }
        }

        private PlayerState GetOwnerState(GameState state, CardInstance source)
        {
            // Проверяем: совпадает ли ID владельца карты с ID профиля игрока?
            if (state.Player.Profile.Id == source.OwnerId)
            {
                return state.Player;
            }

            // Если не совпал, значит карта принадлежит врагу
            return state.Enemy;
        }
    }
}

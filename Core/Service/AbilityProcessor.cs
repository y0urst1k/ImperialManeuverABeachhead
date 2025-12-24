using Core.States;
using Data.Enums;
using Data.Models;
using Services.Interface;

namespace Core.Service
{
    public class AbilityProcessor
    {
        private readonly TargetResolver _targetResolver;

        public AbilityProcessor()
        {
            _targetResolver = new TargetResolver();
        }

        public void Process(GameState state, AbilityModel ability, CardInstance source, ICardDatabaseService _cardDb)
        {
            if (!CheckCondition(state, ability, source))
                return;

            // 2. Потом цели
            List<CardInstance> targets;

            if (ability.TargetSelector != null)
            {
                targets = _targetResolver.GetTargets(state, ability.TargetSelector, source);
            }
            else
            {
                // Fallback для старого TargetType (если нужно)
                targets = GetTargetsLegacy(...);
            }

            // 3. Эффект
            foreach (var target in targets)
            {
                ApplyEffect(target, ability.Effect, ability.Value);
            }

            // 4. Глобальные эффекты (которые не требуют цели, например Summon или AddResource)
            ApplyGlobalEffect(state, ability, source, _cardDb);
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
            if (state.Player.PlayerId == source.OwnerId)
            {
                return state.Player;
            }

            // Если не совпал, значит карта принадлежит врагу
            return state.Enemy;
        }

        private void ApplyEffect(GameState state, AbilityModel ability, CardInstance target, CardInstance source, ICardDatabaseService _cardDb)
        {
            switch (ability.Effect)
            {
                case EffectType.Damage:
                    target.CurrentHealth -= ability.Value;
                    CheckDeath(state, target);
                    break;

                case EffectType.BuffAttack:
                    target.CurrentAttack += ability.Value;
                    break;

                    // ... и так далее
            }
        }
    }
}

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

        public bool Process(GameState state, AbilityModel ability, CardInstance source, ICardDatabaseService _cardDb)
        {
            if (!CheckCondition(state, ability, source))
                return false;

            // 2. Обработка целей (Targeted Effects)
            if (ability.Selector != null)
            {
                var targets = _targetResolver.GetTargets(state, ability.Selector, source);

                // Если цели нужны, но их нет — абилка может не сработать (зависит от правил игры)
                if (targets.Count == 0 && ability.Selector.Count > 0) return false;

                foreach (var target in targets)
                {
                    ApplyTargetedEffect(state, ability, target, source);
                }
            }

            // 3. Обработка глобальных эффектов (Non-Targeted)
            ApplyGlobalEffect(state, ability, source, _cardDb);

            return true;
        }

        private void ApplyTargetedEffect(GameState state, AbilityModel ability, CardInstance target, CardInstance source)
        {
            switch (ability.Effect)
            {
                case EffectType.Damage:
                    // Просто меняем цифры. Смерть проверит GameEngine.
                    target.CurrentHealth -= ability.Value;
                    break;

                case EffectType.BuffAttack:
                    target.BuffAttack += ability.Value; // Лучше в модификатор, чем в CurrentAttack
                    break;

                    // Heal, Silence и т.д.
            }
        }

        private void ApplyGlobalEffect(GameState state, AbilityModel ability, CardInstance source, ICardDatabaseService cardDb)
        {
            var owner = GetOwnerState(state, source);

            switch (ability.Effect)
            {
                case EffectType.GenerateResource:
                    owner.CurrentResources += ability.Value;
                    break;

                case EffectType.Summon:
                    if (Guid.TryParse(ability.ConditionParam, out Guid cardId))
                    {
                        var tokenModel = cardDb.GetCardById(cardId);
                        if (tokenModel != null)
                        {
                            var token = new CardInstance
                            {
                                BaseData = tokenModel,
                                CurrentHealth = tokenModel.Health,
                                CurrentAttack = tokenModel.Attack,
                                OwnerId = owner.PlayerId
                            };
                            owner.Frontline.Add(token);
                        }
                    }
                    break;

                case EffectType.DrawCard:
                    // Логика добора (лучше вызывать метод движка DrawCards, но можно и тут)
                    break;
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
    }
}

using Core.States;
using Data.Enums;
using Data.Models;

namespace Core.Service
{
    public class AbilityProcessor
    {
        private readonly GameState _state;

        public AbilityProcessor(GameState state)
        {
            _state = state;
        }

        public void Process(AbilityModel ability, CardInstance source)
        {
            switch (ability.Effect)
            {
                case EffectType.Damage:
                    // Пример: Нанести урон случайному врагу (если цель не выбрана явно)
                    var enemyUnit = _state.Enemy.Frontline.FirstOrDefault();
                    if (enemyUnit != null)
                    {
                        enemyUnit.CurrentHealth -= ability.Value;
                        CheckDeath(enemyUnit);
                    }
                    break;

                case EffectType.BuffAttack:
                    // Пример: Баффнуть себя
                    source.CurrentAttack += ability.Value;
                    break;

                case EffectType.GenerateResource:
                    if (_state.IsPlayerTurn) _state.Player.CurrentResources += ability.Value;
                    else _state.Enemy.CurrentResources += ability.Value;
                    break;
            }
        }

        private void CheckDeath(CardInstance unit)
        {
            if (unit.CurrentHealth <= 0)
            {
                // Перенос в сброс (Graveyard)
                _state.Enemy.Frontline.Remove(unit);
                _state.Enemy.Graveyard.Add(unit);
            }
        }
    }
}

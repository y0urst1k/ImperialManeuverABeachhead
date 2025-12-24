using Core.States;
using Data.Enums;
using Data.Models;

namespace Core.Service
{
    public class TargetResolver
    {
        public List<CardInstance> GetTargets(GameState state, TargetSelector selector, CardInstance source)
        {
            var candidates = new List<CardInstance>();

            // 1. Фильтр по владельцу (Owner)
            var playerState = GetOwnerState(state, source);
            var enemyState = (playerState == state.Player) ? state.Enemy : state.Player;

            if (selector.Owner == OwnerType.Player || selector.Owner == OwnerType.Any)
            {
                candidates.AddRange(playerState.Frontline);
                candidates.AddRange(playerState.Backline);
            }
            if (selector.Owner == OwnerType.Enemy || selector.Owner == OwnerType.Any)
            {
                candidates.AddRange(enemyState.Frontline);
                candidates.AddRange(enemyState.Backline);
            }

            // 2. Фильтр по зоне (Zone)
            if (selector.Zone == ZoneType.Frontline)
                candidates = candidates.Where(c => IsInFrontline(state, c)).ToList();
            else if (selector.Zone == ZoneType.Backline)
                candidates = candidates.Where(c => !IsInFrontline(state, c)).ToList();

            // 3. Фильтр по типу (UnitType)
            if (selector.UnitType == UnitTypeFilter.OnlyUnits)
                candidates = candidates.Where(c => c.BaseData.Type == CardType.Unit || c.BaseData.Type == CardType.Hero).ToList();

            // 4. Выбор конкретного количества (Count & Mode)
            if (selector.Count > 0 && candidates.Count > selector.Count)
            {
                if (selector.Mode == SelectionMode.Random)
                {
                    // Перемешать и взять N
                    var rnd = new Random();
                    return candidates.OrderBy(x => rnd.Next()).Take(selector.Count).ToList();
                }
                // Если Manual - это уже задача UI, движок тут может вернуть всё или ошибку
            }

            return candidates;
        }

        private PlayerState GetOwnerState(GameState state, CardInstance source)
        {
            return (state.Player.PlayerId == source.OwnerId) ? state.Player : state.Enemy;
        }

        private bool IsInFrontline(GameState state, CardInstance c)
        {
            return state.Player.Frontline.Contains(c) || state.Enemy.Frontline.Contains(c);
        }
    }
}

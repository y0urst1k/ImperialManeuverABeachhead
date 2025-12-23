using Data.Enums;
using Data.Models;

namespace MainComponents.ViewModels
{
    public class CardViewModel : BindableBase
    {
        private readonly CardModel _baseStats;

        private int _currentAttack;
        public int CurrentAttack
        {
            get => _currentAttack;
            set => SetProperty(ref _currentAttack, value);
        }

        // Метод, который вызывает Движок
        public void Recalculate(IEnumerable<AbilityModel> activeAuras)
        {
            int attack = _baseStats.Attack;

            foreach (var aura in activeAuras)
            {
                // Если аура применима к этой карте (например, +1 всем пехотинцам)
                if (aura.Effect == EffectType.BuffAttack && this.IsUnit)
                {
                    attack += aura.Value;
                }
            }

            CurrentAttack = attack;
        }
    }
}

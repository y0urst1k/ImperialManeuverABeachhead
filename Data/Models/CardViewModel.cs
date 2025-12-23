namespace MainComponents.ViewModels
{
    public class CardViewModel : BindableBase
    {
        private readonly CardInstance _instance;

        public string Name => _instance.BaseData.Name;
        public int Attack => _instance.CurrentAttack; // Берем динамическое значение
        public int Health => _instance.CurrentHealth;
        public int Cost => _instance.BaseData.Cost;
        public bool IsExhausted => _instance.IsExhausted;

        // Ссылка на ID, чтобы отправить команду в движок
        public Guid InstanceId => _instance.Id;

        public CardViewModel(CardInstance instance)
        {
            _instance = instance;
        }
    }
}

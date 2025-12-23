namespace Data.Models
{
    public class PlayerModel
    {

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public int Health { get; set; } = 30;
        public int CurrentResources { get; set; } = 0;
    }
}
namespace Data.Models
{
    public class DeckModel
    {
        public string Name { get; set; }
        public List<Guid> CardIds { get; set; } = new(); // Храним только ID карт
    }
}

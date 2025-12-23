using Data.Models;

namespace Services.Interface
{
    public interface ICardDatabase
    {
        CardModel GetCardById(string id);
        IEnumerable<CardModel> GetAllCards();
    }

    // Управление колодами игрока (User Data)
    public interface IDeckService
    {
        void SaveDeck(DeckModel deck);
        DeckModel LoadDeck(string deckName);
        IEnumerable<string> GetDeckNames();
    }

    public class DeckModel
    {
        public string Name { get; set; }
        public List<Guid> CardIds { get; set; } = new(); // Храним только ID карт
    }
}

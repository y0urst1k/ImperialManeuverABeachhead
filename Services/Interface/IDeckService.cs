using Data.Models;

namespace Services.Interface
{
    // Управление колодами игрока (User Data)
    public interface IDeckService
    {
        void SaveDeck(DeckModel deck);
        DeckModel LoadDeck(string deckName);
        IEnumerable<string> GetDeckNames();
    }
}
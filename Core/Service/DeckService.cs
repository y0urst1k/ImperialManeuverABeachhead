using Data.Models;
using Services.Interface;

namespace Services.Service
{
    public class DeckService : IDeckService
    {
        public IEnumerable<string> GetDeckNames()
        {
            throw new NotImplementedException();
        }

        public DeckModel LoadDeck(string deckName)
        {
            throw new NotImplementedException();
        }

        public void SaveDeck(DeckModel deck)
        {
            throw new NotImplementedException();
        }
    }
}
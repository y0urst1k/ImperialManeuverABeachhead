using Data.Models;
using Services.Interface;

namespace Services.Service
{
    public class CardDatabaseService : ICardDatabaseService
    {
        public IEnumerable<CardModel> GetAllCards()
        {
            throw new NotImplementedException();
        }

        public CardModel GetCardById(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
using Data.Models;

namespace Services.Interface
{
    public interface ICardDatabaseService
    {
        CardModel GetCardById(string id);
        IEnumerable<CardModel> GetAllCards();
    }
}

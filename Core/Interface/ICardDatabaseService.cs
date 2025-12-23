using Data.Models;

namespace Services.Interface
{
    public interface ICardDatabaseService
    {
        CardModel GetCardById(Guid id);
        IEnumerable<CardModel> GetAllCards();
    }
}

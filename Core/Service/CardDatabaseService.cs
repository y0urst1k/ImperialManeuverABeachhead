using System.IO;
using System.Text.Json;
using Data.Models;
using Services.Interface;

namespace Services.Service
{
    public class CardDatabaseService : ICardDatabaseService
    {
        private Dictionary<Guid, CardModel> _cache;
        private const string FilePath = "Data/cards.json";

        public CardDatabaseService()
        {
            LoadData();
        }

        private void LoadData()
        {
            if (!File.Exists(FilePath))
            {
                _cache = new Dictionary<Guid, CardModel>();
                return;
            }

            var json = File.ReadAllText(FilePath);
            var cards = JsonSerializer.Deserialize<List<CardModel>>(json);
            _cache = cards.ToDictionary(x => x.Id);
        }

        public CardModel GetCardById(Guid id) => _cache.GetValueOrDefault(id);
        public IEnumerable<CardModel> GetAllCards() => _cache.Values;
    }
}
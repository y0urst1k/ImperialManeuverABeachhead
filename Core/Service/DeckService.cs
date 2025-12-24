using System.IO;
using System.Text.Json;
using Data.Models;
using Services.Interface;

namespace Services.Service
{
    public class DeckService : IDeckService
    {
        private const string FolderPath = "Data/Decks";

        public DeckService()
        {
            Directory.CreateDirectory(FolderPath);
        }

        public void SaveDeck(DeckModel deck)
        {
            var json = JsonSerializer.Serialize(deck);
            var path = Path.Combine(FolderPath, $"{deck.Name}.json");
            File.WriteAllText(path, json);
        }

        public DeckModel LoadDeck(string deckName)
        {
            var path = Path.Combine(FolderPath, $"{deckName}.json");
            if (!File.Exists(path)) return null;

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<DeckModel>(json);
        }

        public IEnumerable<string> GetDeckNames()
        {
            return Directory.GetFiles(FolderPath, "*.json")
                            .Select(Path.GetFileNameWithoutExtension);
        }
    }
}
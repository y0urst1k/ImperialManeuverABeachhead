using Data.Models;

namespace Core.Interface
{
    public interface ISessionContextService
    {
        PlayerModel CurrentPlayer { get; }
        DeckModel ActiveDeck { get; set; }
    }
}

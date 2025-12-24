using Core.Interface;
using Data.Models;

namespace Core.Service
{
    public class SessionContextService : ISessionContextService
    {
        public PlayerModel CurrentPlayer { get; } = new PlayerModel { Name = "Gamer", Id = Guid.NewGuid() };
        public DeckModel ActiveDeck { get; set; }
    }
}

using PokerBoom.Shared.Models;

namespace PokerBoom.Server.Repositories
{
    public interface IGameRepository
    {
        public Task<IEnumerable<GameReview>> GetGamesList();
    }
}

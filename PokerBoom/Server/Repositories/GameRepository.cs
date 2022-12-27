using Microsoft.EntityFrameworkCore;
using PokerBoom.Server.Data;
using PokerBoom.Shared.Models;

namespace PokerBoom.Server.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly AppDbContext _db;
        public GameRepository(AppDbContext db) 
        {
            _db = db;
        }

        public async Task<IEnumerable<GameReview>> GetGames()
        {
            var games = new List<GameReview>();
            try
            {
                //var g = _db.Games.Include(g => g.Table).ToList();
                _db.ChangeTracker.LazyLoadingEnabled = false;

                foreach (var game in _db.Games.Include(g => g.Table).Include(g => g.Players))
                {
                    games.Add(new GameReview
                    {
                        Id = game.Id,
                        Players = game.Players.Count(),
                        SmallBlind = game.Table.SmallBlind,
                        TableName = game.Table.Name
                    });
                }
            }
            catch(Exception ex)
            {

            }
            return games;
        }
    }
}

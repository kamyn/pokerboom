using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokerBoom.Server.Data;
using PokerBoom.Server.Repositories;
using PokerBoom.Shared.Models;

namespace PokerBoom.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public GamesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<GetGamesResultViewModel>> GetGames()
        {
            //var games = new List<GameReview>();
            //foreach (var game in _db.Games)
            //    games.Add(new GameReview { Id = game.Id, 
            //        Players = _db.Players.Where(p => p.GameId ==  game.Id).Count(),
            //        SmallBlind = game.Table.SmallBlind, 
            //        TableName = game.Table.Name });
            return Ok(new GetGamesResultViewModel { Successful = true, Games = new List<GameReview>() });
        }
    }
}

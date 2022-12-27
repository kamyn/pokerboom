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
        private readonly IGameRepository _gameRepository;
        public GamesController(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        [HttpGet]
        public async Task<ActionResult<GetGamesResultViewModel>> GetGames()
        {
            var games = await _gameRepository.GetGames();
            return Ok(new GetGamesResultViewModel { Successful = true, Games = games });
        }
    }
}

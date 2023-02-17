using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using PokerBoom.Server.Data;
using PokerBoom.Server.Models;
using PokerBoom.Server.Repositories;
using System.Text.Json;
using PokerBoom.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Routing;
using Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace PokerBoom.Server.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GameReviewHub : Hub
    {
        private readonly UserManager<Entities.ApplicationUser> _userManager;
        private AppDbContext _db;

        public GameReviewHub(AppDbContext db)
        {
            _db = db;
        }

        public async Task GetGame(int gameId)
        {
            _db.Players.Include(p => p.User);
            var game = _db.Games.Where(g => g.Id == gameId).Include(g => g.Bets).
                                                            Include(g => g.Players).
                                                            ThenInclude(p => p.User).
                                                            Include(g => g.Board).First();
//          var user = _db.Games.Where(g => g.Id == gameId).First().Players; 
            var bets = game.Bets.OrderBy(b => b.Id);
            var gameInformation = new ReviewGameInformation();
            foreach(var player in game.Players)
            {
                gameInformation.Players.Add(new ReviewGamePlayer
                {
                    HandCards = new List<int> { player.FirstCard, player.SecondCard },
                    IsPlaying = true,
                    SeatNumber = player.SeatPlace,
                    Stack = player.Stack,
                    Username = player.User.UserName
                });
            }
            foreach(var bet in bets)
            {
                switch(bet.Round)
                {
                    case 1: 
                        if (gameInformation.TableCards.Count == 0)
                            gameInformation.TableCards.AddRange(new List<int> { game.Board.Card1, game.Board.Card2, game.Board.Card3 });break;
                    case 2: 
                        if (gameInformation.TableCards.Count == 3)
                            gameInformation.TableCards.Add(game.Board.Card4); break;
                    case 3: 
                        if (gameInformation.TableCards.Count == 4)
                            gameInformation.TableCards.Add(game.Board.Card5); break;
                }
                //if (bet.BetAmount < 0)
                //{
                //    ...
                //}
                gameInformation.CurrentPlayer = bet.Player.User.UserName;
                gameInformation.Players.Where(p => p.Username == gameInformation.CurrentPlayer).First().Bet = bet.BetAmount;
                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveGameInformation", JsonSerializer.Serialize(gameInformation));
                await Task.Delay(2000);
            }
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using PokerBoom.Server.Data;
using PokerBoom.Server.Models;
using PokerBoom.Server.Repositories;
using System.Text.Json;
using PokerBoom.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace PokerBoom.Server.Hubs
{
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
            //while (true)
            //{
            var game = _db.Games.Where(g => g.Id == gameId).Include(g => g.Bets).Include(g => g.Players).Include(g => g.Board).First();
            
            foreach(var bet in game.Bets)
            {
                //var gameInfo = new ReviewGameInformation
                //{
                //    Players = new List<ReviewGamePlayer> 
                //}
            }
            
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveGameInformation", JsonSerializer.Serialize(new ReviewGameInformation
            {
                Players = new List<ReviewGamePlayer>
                {
                    new ReviewGamePlayer { Username = "pavel", HandCards = new List<int> { 1, 2}, IsPlaying = true, SeatNumber = 1, Stack = 100 },
                    new ReviewGamePlayer { Username = "root", HandCards = new List<int> { 3, 4, }, IsPlaying = true, SeatNumber = 2, Stack = 300 },
                    new ReviewGamePlayer { Username = "root1", HandCards = new List<int> { 42, 9, }, IsPlaying = true, SeatNumber = 3, Stack = 300 },
                    new ReviewGamePlayer { Username = "root2", HandCards = new List<int> { 24, 17, }, IsPlaying = true, SeatNumber = 4, Stack = 300 },
                    new ReviewGamePlayer { Username = "root3", HandCards = new List<int> { 38, 36, }, IsPlaying = true, SeatNumber = 5, Stack = 300 },
                    new ReviewGamePlayer { Username = "root4", HandCards = new List<int> { 20, 11, }, IsPlaying = true, SeatNumber = 6, Stack = 300 }
                },
                Pot = 0,
                TableCards = new List<int> { 5, 6, 7},
            }));
            await Task.Delay(3000);
            //}
        }
    }
}

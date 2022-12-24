using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using PokerBoom.Server.Data;
using PokerBoom.Server.Models;
using PokerBoom.Server.Repositories;
using PokerBoom.Server.Static;
using PokerBoom.Shared.Models;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace PokerBoom.Server.Hubs
{
    public class GameHub : Hub
    {
        private readonly UserManager<Entities.ApplicationUser> _userManager;
        private readonly ITableRepository _tableRepository;
        private AppDbContext _db;

        public static List<Game> Games { get; set; } = new List<Game>();
        public static List<User> Users { get; set; } = new List<User>();

        public GameHub(UserManager<Entities.ApplicationUser> userManager, ITableRepository tableRepository, AppDbContext db)
        {
            _userManager = userManager;
            _tableRepository = tableRepository;
            _db = db;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            
        }

        private async Task StartNewGame(int tableId, int smallBlind, int smallBlindIndex)
        {
            var game = new Game(tableId, smallBlind, smallBlindIndex); 
            var users = Users.Where(u => u.TableId == tableId).ToList();
            for(int i = 0; i < users.Count; ++i)
            {
                var player = new Player
                {
                    Name = users[i].Name,
                    HandCards = game.Deck.NextCards(2),
                    SeatNumber = users[i].SeatNumber,
                    Stack = users[i].Stack,
                    IsPlaying = true,
                    RoundBet = users[i].SeatNumber == smallBlindIndex ? smallBlind : 0,
                    HasMadeMove = users[i].SeatNumber == smallBlindIndex,
                };
                game.Players.Add(player);
            }
            game.BetRound = Round.PreFlop;
            game.Pot = smallBlind;
            game.CurrentPlayer = game.Players.OrderBy(p => p.SeatNumber).ToList()[1].Name;

            for (int i = 0; i < users.Count; ++i)
                await Clients.Client(users[i].ConnectionId).SendAsync("ReceiveHandCards", JsonSerializer.Serialize(game.Players[i].HandCards));

            Games.Add(game);
            await SendGameInformation(tableId);

            game.Id = _db.Games.Count() == 0 ? 1 : _db.Games.OrderBy(g => g.Id).Last().Id + 1;
            try
            {
                _db.Games.Add(new Entities.Game
                {
                    Id = game.Id,
                    TableId = 1, // todo
                    Players = new List<Entities.Player>(),
                    Board = new Entities.Board(),
                    Bets = new List<Entities.Bet>()
                });

                for (int i = 0; i < users.Count; ++i)
                {
                    _db.Players.Add(new Entities.Player
                    {
                        FirstCard = game.Players[i].HandCards[0],
                        SecondCard = game.Players[i].HandCards[1],
                        SeatPlace = game.Players[i].SeatNumber,
                        Stack = game.Players[i].Stack,
                        UserId = _db.Users.First(u => u.UserName == users[i].Name).Id,
                        GameId = game.Id
                    });
                }
                _db.SaveChanges();
                _db.Bets.Add(new Entities.Bet
                {
                    BetAmount = smallBlind,
                    Player = _db.Players.First(p => p.SeatPlace == smallBlindIndex && p.GameId == game.Id),
                    Round = 0,
                    Game = _db.Games.First(g => g.Id == game.Id)
                });
            }
            catch (Exception ex)
            {

            }
            _db.SaveChanges();
        }

        private async Task SendGameInformation(int tableId)
        {
            var gamePlayers = new List<GamePlayer>();
            var game = Games.FirstOrDefault(g => g.TableId == tableId);
            if (game != null)
            {
                for (int i = 0; i < game.Players.Count(); ++i)
                {
                    gamePlayers.Add(new GamePlayer
                    {
                        Username = game.Players[i].Name,
                        SeatNumber = game.Players[i].SeatNumber,
                        Stack = game.Players[i].Stack, // TODO переложить стек на юзера
                        IsPlaying = game.Players[i].IsPlaying,
                        Bet = game.Players[i].RoundBet
                    });
                }

                await Clients.Groups(tableId.ToString()).SendAsync("ReceiveGameInformation", JsonSerializer.Serialize(new GameInformation
                {
                    CurrentPlayer = game.CurrentPlayer,
                    TableCards = game.TableCards,
                    Winners = game.Winners,
                    Players = gamePlayers,
                    RoundRaiseAmount = game.RoundRaiseAmount,
                    Pot = game.Pot
                }));

                // write to db
            }
            else
            {
                await Clients.Groups(tableId.ToString()).SendAsync("ReceiveKick");
            }
        }

        private async Task NominateWinners(Game game)
        {
            var players = game.Players.Where(p => p.IsPlaying);

            foreach (var player in players)
                player.HandStrength = PokerEvaluator.GetHandStrength(game.TableCards.Concat(player.HandCards).ToList());

            var winners = players.Where(p => p.HandStrength == players.Max(p => p.HandStrength)).ToList();
            int winnersCount = winners.Count;

            foreach (var winner in winners)
            {
                game.Winners.Add(((Player)winner).Name);
            }

            foreach(var winner in game.Winners)
            {
                var user = Users.FirstOrDefault(u => u.Name == winner);
                if (user != null)
                {
                    user.Stack += game.Pot / winnersCount;

                    var dbUser = await _userManager.FindByNameAsync(user.Name);
                    dbUser.Currency += game.Pot / winnersCount;
                    await _userManager.UpdateAsync(dbUser);
                }
            }

            game.CurrentPlayer = "";

            await SendGameInformation(game.TableId);

            int tableId = game.TableId;
            int smallBlind = game.SmallBlind;
            int smallBlindIndex = 1;

            Games.Remove(game);
            await Task.Delay(5000);
            await StartNewGame(tableId, smallBlind, smallBlindIndex);
        }

        private async Task<int> GetFreeSeat(int tableId)
        {
            var users = Users.Where(u => u.TableId == tableId).ToList();
            if (users != null && users.Count < 6)
            {
                if (users.Count == 0)
                    return 1;
                if (users.Max(u => u.SeatNumber) < 6)
                    return users.Last().SeatNumber + 1;
                else
                {
                    foreach(var u in users)
                    {
                        var user = users.FirstOrDefault(x => x.SeatNumber == u.SeatNumber + 1);
                        if (user == null)
                            return u.SeatNumber + 1;
                    }
                }
            }
            return 1;
        }
        public async Task AddToUsers(KeyValuePair<int,int> playerInfo)
        {
            int tableId = playerInfo.Key;
            int stack = playerInfo.Value;
            var table = await _tableRepository.GetTableById(tableId);

            // если достаточно мест за столом, проверка баланса
            var users = Users.Where(u => u.TableId == tableId).ToList();
            int players = users.Count;
            if (players < 6 && Users.Where(u => u.Name == Context.User.Identity.Name).Count() == 0)
            {
                var user = new User
                {
                    Balance = stack, // TODO брать из userManager
                    ConnectionId = Context.ConnectionId,
                    InGame = true,
                    Name = Context.User.Identity.Name,
                    TableId = tableId,
                    SeatNumber = await GetFreeSeat(tableId)
                };

                Users.Add(user);
                await Groups.AddToGroupAsync(Context.ConnectionId, tableId.ToString());
                var game = Games.FirstOrDefault(g => g.TableId == tableId);
                if (game == null && players > 0)
                    await StartNewGame(tableId, table.SmallBlind, 1);
            }
            else 
                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveKick");
        }

        public async Task ActionCheck()
        {
            var user = Users.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            if (user != null)
            {
                var game = Games.FirstOrDefault(g => g.TableId == user.TableId);
                if (game != null)
                {
                    var player = game.Players.FirstOrDefault(p => p.Name == Context.User.Identity.Name);
                    if (player != null)
                    {
                        player.HasMadeMove = true;

                        _db.Bets.Add(new Entities.Bet
                        {
                            BetAmount = 0,
                            Game = _db.Games.First(g => g.Id == game.Id),
                            Player = _db.Players.First(p => p.SeatPlace == player.SeatNumber && p.GameId == game.Id),
                            Round = (int)game.BetRound,
                        });
                        _db.SaveChanges();
                    }

                    if (game.Players.Where(p => p.IsPlaying).All(p => p.RoundBet == game.Players.Max(p => p.RoundBet)) &&
                        game.Players.Where(p => p.IsPlaying).All(p => p.HasMadeMove))
                    {
                        game.BetRound = game.BetRound + 1;
                        switch(game.BetRound)
                        {
                            case Round.Flop: game.TableCards.AddRange(game.Deck.NextCards(3)); break;
                            case Round.Turn: game.TableCards.Add(game.Deck.NextCard()); break;
                            case Round.River: game.TableCards.Add(game.Deck.NextCard()); break;
                            case Round.PostRiver: await NominateWinners(game); return;
                        }

                        foreach (var p in game.Players)
                        {
                            p.HasMadeMove = false;
                            p.RoundBet = 0;
                        }
                        game.RoundRaiseAmount = 0;
                    }
                    game.Players = game.Players.OrderBy(p => p.SeatNumber).ToList();
                    int idxNextPlayer = (game.Players.FindIndex(p => p.Name == game.CurrentPlayer) + 1) % game.Players.Count();
                    game.CurrentPlayer = game.Players[idxNextPlayer].Name;
                    await SendGameInformation(user.TableId);
                }
            }
        }

        public async Task ActionCall()
        {
            var user = Users.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            if (user != null)
            {
                var game = Games.FirstOrDefault(g => g.TableId == user.TableId);
                if (game != null)
                {
                    game.Players = game.Players.OrderBy(p => p.SeatNumber).ToList();

                    int idxPrevPlayer = (game.Players.FindIndex(p => p.Name == game.CurrentPlayer) + 5) % 6;
                    var prevPlayer = game.Players[idxPrevPlayer];
                    var player = game.Players.FirstOrDefault(p => p.Name == Context.User.Identity.Name);
                    
                    if (prevPlayer != null && player != null)
                    {
                        user.Stack -= prevPlayer.RoundBet;
                        player.RoundBet = prevPlayer.RoundBet;
                        player.Stack -= prevPlayer.RoundBet;
                        player.HasMadeMove = true;
                        game.Pot += prevPlayer.RoundBet;

                        _db.Bets.Add(new Entities.Bet
                        {
                            BetAmount = prevPlayer.RoundBet,
                            Game = _db.Games.First(g => g.Id == game.Id),
                            Player = _db.Players.First(p => p.SeatPlace == player.SeatNumber && p.GameId == game.Id),
                            Round = (int)game.BetRound,
                        });
                        _db.SaveChanges();
                    }

                    if (game.Players.Where(p => p.IsPlaying).All(p => p.RoundBet == game.Players.Max(p => p.RoundBet)) &&
                        game.Players.Where(p => p.IsPlaying).All(p => p.HasMadeMove))
                    {
                        game.BetRound = game.BetRound + 1;
                        switch (game.BetRound)
                        {
                            case Round.Flop: game.TableCards.AddRange(game.Deck.NextCards(3)); break;
                            case Round.Turn: game.TableCards.Add(game.Deck.NextCard()); break;
                            case Round.River: game.TableCards.Add(game.Deck.NextCard()); break;
                            case Round.PostRiver: await NominateWinners(game); return;
                        }

                        foreach (var p in game.Players)
                        {
                            p.HasMadeMove = false;
                            p.RoundBet = 0;
                        }
                        game.RoundRaiseAmount = 0;
                    }

                    int idxNextPlayer = (game.Players.FindIndex(p => p.Name == game.CurrentPlayer) + 1) % game.Players.Count();
                    game.CurrentPlayer = game.Players[idxNextPlayer].Name;
                    await SendGameInformation(user.TableId);
                }
            }
        }

        public async Task ActionRaise(int amount)
        {
            var user = Users.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            if (user != null)
            {
                var game = Games.FirstOrDefault(g => g.TableId == user.TableId);
                if (game != null)
                {
                    game.Players = game.Players.OrderBy(p => p.SeatNumber).ToList();
                    
                    if (amount <= game.Players.Where(p => p.IsPlaying).Min(p => p.Stack))
                    {
                        game.RoundRaiseAmount= amount;
                        var player = game.Players.FirstOrDefault(p => p.Name == Context.User.Identity.Name);
                        if (player != null)
                        {
                            user.Stack -= amount;
                            player.RoundBet = amount;
                            player.Stack -= amount;
                            player.HasMadeMove = true;
                        }
                        //TODO учитывать неактивных игроков (isplaying = false)
                        int idxNextPlayer = (game.Players.FindIndex(p => p.Name == game.CurrentPlayer) + 1) % game.Players.Count();
                        game.CurrentPlayer = game.Players[idxNextPlayer].Name;
                        await SendGameInformation(user.TableId);
                    }
                }
            }
        }

        public async Task ActionAllIn()
        {
            var user = Users.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            if (user != null)
            {
                var game = Games.FirstOrDefault(g => g.TableId == user.TableId);
                if (game != null)
                {
                    game.Players = game.Players.OrderBy(p => p.SeatNumber).ToList();

                    int amount = game.Players.Where(p => p.IsPlaying).Min(p => p.Stack);
                    game.RoundRaiseAmount = amount;
                    var player = game.Players.FirstOrDefault(p => p.Name == Context.User.Identity.Name);
                    if (player != null)
                    {
                        user.Stack -= amount;
                        player.RoundBet = amount;
                        player.Stack -= amount;
                        player.HasMadeMove = true;
                    }
                    //TODO учитывать неактивных игроков (isplaying = false)
                    int idxNextPlayer = (game.Players.FindIndex(p => p.Name == game.CurrentPlayer) + 1) % game.Players.Count();
                    game.CurrentPlayer = game.Players[idxNextPlayer].Name;
                    await SendGameInformation(user.TableId);
                }
            }
        }

        public async Task ActionFold()
        {
            var player = Users.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            if (player != null)
            {
                // если игрок остался один, то он победитель; isplaying = false
                await SendGameInformation(player.TableId);
            }
        }
    }
}

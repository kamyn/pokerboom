using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PokerBoom.Server.Data;
using PokerBoom.Server.Models;
using PokerBoom.Server.Repositories;
using PokerBoom.Server.Static;
using PokerBoom.Shared.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PokerBoom.Server.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        public async Task AddToUsers(KeyValuePair<int, int> playerInfo)
        {
            int tableId = playerInfo.Key;
            int stack = playerInfo.Value;
            var table = await _tableRepository.GetTableById(tableId);
            
            if (Context.User?.Identity?.Name != null)
            {
                var userName = Context.User.Identity.Name;
                var userInfo = await _userManager.FindByNameAsync(userName);
                if (stack <= userInfo.Currency)
                {
                    var users = Users.Where(u => u.TableId == tableId).ToList();
                    int players = users.Count;
                    if (players < 6 && Users.Where(u => u.Name == userName).Count() == 0)
                    {
                        userInfo.Currency -= stack;
                        await _userManager.UpdateAsync(userInfo);
                        var user = new User
                        {
                            ConnectionId = Context.ConnectionId,
                            InGame = true,
                            Name = userName,
                            TableId = tableId,
                            SeatNumber = await GetFreeSeat(tableId),
                            Stack = stack
                        };
                        Users.Add(user);
                        await Groups.AddToGroupAsync(Context.ConnectionId, tableId.ToString());
                        await _tableRepository.SetPlayers(tableId, players + 1);
                        var game = Games.FirstOrDefault(g => g.TableId == tableId);
                        if (game == null && players > 0)
                            await StartNewGame(tableId, table.SmallBlind, 1);
                    }
                    else
                        await Clients.Client(Context.ConnectionId).SendAsync("ReceiveKick");
                }
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (Context.User?.Identity?.Name != null)
            {
                var userName = Context.User.Identity.Name;
                var userInfo = await _userManager.FindByNameAsync(userName);
                var user = Users.First(u => u.Name == userName);
                if (user != null)
                {
                    userInfo.Currency += user.Stack;
                    await _userManager.UpdateAsync(userInfo);
                    Users.Remove(user);
                    var game = Games.First(g => g.TableId == user.TableId);
                    if (game != null)
                    {
                        var player = game.Players.FirstOrDefault(p => p.Name == user.Name);
                        if (player != null)
                        {
                            player.IsPlaying = false;
                            game.Players.Remove(player);
                            if (game.Players.Where(p => p.IsPlaying).Count() < 2)
                                await NominateWinners(game);
                            await _tableRepository.SetPlayers(user.TableId, game.Players.Count());
                            await SendGameInformation(user.TableId);
                        }
                    }
                }
            }
        }

        private async Task StartNewGame(int tableId, int smallBlind, int smallBlindIndex)
        {
            var game = new Game(tableId, smallBlind, smallBlindIndex); 
            var users = Users.Where(u => u.TableId == tableId).ToList();

            var table = _tableRepository.GetTableById(tableId);
            if (table.Id == -1)
            {
                for (int i = 0; i < users.Count; ++i)
                    await Clients.Client(users[i].ConnectionId).SendAsync("ReceiveKick");
                return;
            }

            for (int i = 0; i < users.Count; ++i)
            {
                users[i].Stack = users[i].Stack - (users[i].SeatNumber == smallBlindIndex ? smallBlind : 0);
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
                await Clients.Client(users[i].ConnectionId).SendAsync("ReceiveHandCards", JsonSerializer.Serialize(player.HandCards));
            }
            game.BetRound = Round.PreFlop;
            game.Pot = smallBlind;
            game.CurrentPlayer = game.Players.OrderBy(p => p.SeatNumber).ToList()[1].Name;
            game.RoundRaiseAmount = smallBlind;

            Games.Add(game);
            await SendGameInformation(tableId);

            game.Id = _db.Games.Count() == 0 ? 1 : _db.Games.OrderBy(g => g.Id).Last().Id + 1; 
            try
            {
                _db.Games.Add(new Entities.Game
                {
                    Id = game.Id,
                    TableId = tableId,
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
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("failed to start new game");
            }
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
                        Stack = game.Players[i].Stack,
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
            }
            else
            {
                await Clients.Groups(tableId.ToString()).SendAsync("ReceiveKick");
            }
        }

        private async Task NominateWinners(Game game)
        {
            if (game.Players.Where(p => p.IsPlaying).Count() < 2)
            {
                var player = game.Players.FirstOrDefault(p => p.IsPlaying);
                if (player != null)
                {
                    game.Winners.Add(((Player)player).Name);
                    var user = Users.FirstOrDefault(u => u.Name == player.Name);
                    if (user != null)
                        user.Stack += game.Pot;
                    game.CurrentPlayer = "";
                    await SendGameInformation(game.TableId);
                    Games.Remove(game);
                    if (game.Players.Count > 1)
                    {
                        await Task.Delay(5000);
                        await StartNewGame(game.TableId, game.SmallBlind, 1);
                        return;
                    }
                    var userInfo = await _userManager.FindByNameAsync(player.Name);
                    if (user != null)
                    {
                        userInfo.Currency += user.Stack;
                        await _userManager.UpdateAsync(userInfo);
                        Users.Remove(user);
                    }
                    _tableRepository.SetPlayers(game.TableId, 0);
                    return;
                }
            }
            var players = game.Players.Where(p => p.IsPlaying);

            foreach (var p in players)
                p.HandStrength = PokerEvaluator.GetHandStrength(game.TableCards.Concat(p.HandCards).ToList());

            var winners = players.Where(p => p.HandStrength == players.Max(p => p.HandStrength)).ToList();
            int winnersCount = winners.Count;

            foreach (var winner in winners)
            {
                game.Winners.Add(((Player)winner).Name);
            }

            foreach (var winner in game.Winners)
            {
                var user = Users.FirstOrDefault(u => u.Name == winner);
                if (user != null)
                    user.Stack += game.Pot / winnersCount;
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

        public async Task ActionCheck()
        {
            if (Context.User?.Identity?.Name != null)
            {
                var userName = Context.User.Identity.Name;
                var user = Users.FirstOrDefault(u => u.Name == userName);
                if (user != null)
                {
                    var game = Games.FirstOrDefault(g => g.TableId == user.TableId);
                    if (game != null)
                    {
                        var player = game.Players.FirstOrDefault(p => p.Name == userName);
                        var prevPlayer = game.Players.Where(p => p.IsPlaying).
                                OrderByDescending(p => p.SeatNumber).
                                First(p => p.SeatNumber -
                                        (game.Players.Where(p => p.IsPlaying).Min(p => p.SeatNumber) == user.SeatNumber ? 6 : 0)
                                < user.SeatNumber);
                        if (player != null)
                        {
                            if (player.RoundBet == prevPlayer.RoundBet)
                                player.HasMadeMove = true;
                            _db.Bets.Add(new Entities.Bet
                            {
                                BetAmount = 0,
                                Game = _db.Games.First(g => g.Id == game.Id),
                                Player = _db.Players.First(p => p.SeatPlace == player.SeatNumber && p.GameId == game.Id),
                                Round = (int)game.BetRound,
                            });
                            _db.SaveChanges();
                            if (game.Players.Where(p => p.IsPlaying).All(p => p.RoundBet == game.Players.Max(p => p.RoundBet)) &&
                                game.Players.Where(p => p.IsPlaying).All(p => p.HasMadeMove))
                            {
                                game.BetRound = game.BetRound + 1;
                                switch (game.BetRound)
                                {
                                    case Round.Flop: 
                                        game.TableCards.AddRange(game.Deck.NextCards(3));
                                        var dbBoard = _db.Games.Where(g => g.Id == game.Id).Include(g => g.Board).First().Board;
                                        dbBoard.Card1 = game.TableCards[0];
                                        dbBoard.Card2 = game.TableCards[1];
                                        dbBoard.Card3 = game.TableCards[2];
                                        _db.SaveChanges();
                                        break;
                                    case Round.Turn: 
                                        game.TableCards.Add(game.Deck.NextCard());
                                        _db.Games.Where(g => g.Id == game.Id).Include(g => g.Board).First().Board.Card4 = game.TableCards[3];
                                        _db.SaveChanges();
                                        break;
                                    case Round.River:
                                        game.TableCards.Add(game.Deck.NextCard());
                                        _db.Games.Where(g => g.Id == game.Id).Include(g => g.Board).First().Board.Card5 = game.TableCards[4];
                                        _db.SaveChanges();
                                        break;
                                    case Round.PostRiver: await NominateWinners(game); return;
                                }
                                foreach (var p in game.Players)
                                {
                                    p.HasMadeMove = false;
                                    p.RoundBet = 0;
                                }
                                game.RoundRaiseAmount = 0;
                            }

                            if (player.RoundBet == prevPlayer.RoundBet)
                                game.CurrentPlayer = game.Players.Where(p => p.IsPlaying).
                                    OrderBy(p => p.SeatNumber).
                                    First(p => p.SeatNumber +
                                            (game.Players.Where(p => p.IsPlaying).Max(p => p.SeatNumber) == user.SeatNumber ? 6 : 0)
                                    > user.SeatNumber).Name;

                            await SendGameInformation(user.TableId);
                        }
                    }
                }
            }
        }

        public async Task ActionCall()
        {
            if (Context.User?.Identity?.Name != null)
            {
                var userName = Context.User.Identity.Name;
                var user = Users.FirstOrDefault(u => u.Name == userName);
                if (user != null)
                {
                    var game = Games.FirstOrDefault(g => g.TableId == user.TableId);
                    if (game != null)
                    {
                        game.Players = game.Players.OrderBy(p => p.SeatNumber).ToList();
                        try
                        {
                            var prevPlayer = game.Players.Where(p => p.IsPlaying).
                            OrderBy(p => p.SeatNumber).
                            First(p => p.SeatNumber +
                                    (game.Players.Where(p => p.IsPlaying).Max(p => p.SeatNumber) == user.SeatNumber ? 6 : 0)
                            > user.SeatNumber);
                            var player = game.Players.FirstOrDefault(p => p.Name == userName);
                            if (prevPlayer != null && player != null)
                            {
                                user.Stack = (user.Stack - game.RoundRaiseAmount + player.RoundBet);
                                player.Stack = (player.Stack - game.RoundRaiseAmount + player.RoundBet);
                                game.Pot += (game.RoundRaiseAmount - player.RoundBet);
                                player.RoundBet = prevPlayer.RoundBet;
                                player.HasMadeMove = true;

                                _db.Bets.Add(new Entities.Bet
                                {
                                    BetAmount = prevPlayer.RoundBet,
                                    Game = _db.Games.First(g => g.Id == game.Id),
                                    Player = _db.Players.First(p => p.SeatPlace == player.SeatNumber && p.GameId == game.Id),
                                    Round = (int)game.BetRound,
                                });
                                _db.SaveChanges();
                            }
                        }
                        catch (Exception ex)
                        {
                            var msg = ex.Message;
                        }

                        if (game.Players.Where(p => p.IsPlaying).All(p => p.RoundBet == game.Players.Max(p => p.RoundBet)) &&
                            game.Players.Where(p => p.IsPlaying).All(p => p.HasMadeMove))
                        {
                            game.BetRound = game.BetRound + 1;
                            switch (game.BetRound)
                            {
                                case Round.Flop:
                                    game.TableCards.AddRange(game.Deck.NextCards(3));
                                    var dbBoard = _db.Games.Where(g => g.Id == game.Id).Include(g => g.Board).First().Board;
                                    dbBoard.Card1 = game.TableCards[0];
                                    dbBoard.Card2 = game.TableCards[1];
                                    dbBoard.Card3 = game.TableCards[2];
                                    _db.SaveChanges();
                                    break;
                                case Round.Turn:
                                    game.TableCards.Add(game.Deck.NextCard());
                                    _db.Games.Where(g => g.Id == game.Id).Include(g => g.Board).First().Board.Card4 = game.TableCards[3];
                                    _db.SaveChanges();
                                    break;
                                case Round.River:
                                    game.TableCards.Add(game.Deck.NextCard());
                                    _db.Games.Where(g => g.Id == game.Id).Include(g => g.Board).First().Board.Card5 = game.TableCards[4];
                                    _db.SaveChanges();
                                    break;
                                case Round.PostRiver: await NominateWinners(game); return;
                            }

                            foreach (var p in game.Players)
                            {
                                p.HasMadeMove = false;
                                p.RoundBet = 0;
                            }
                            game.RoundRaiseAmount = 0;
                        }

                        game.CurrentPlayer = game.Players.Where(p => p.IsPlaying).
                            OrderBy(p => p.SeatNumber).
                            First(p => p.SeatNumber +
                                    (game.Players.Where(p => p.IsPlaying).Max(p => p.SeatNumber) == user.SeatNumber ? 6 : 0)
                            > user.SeatNumber).Name;
                        await SendGameInformation(user.TableId);
                    }
                }
            }
        }

        public async Task ActionRaise(int amount)
        {
            if (Context.User?.Identity?.Name != null)
            {
                var userName = Context.User.Identity.Name;
                var user = Users.FirstOrDefault(u => u.Name == userName);
                if (user != null)
                {
                    var game = Games.FirstOrDefault(g => g.TableId == user.TableId);
                    if (game != null)
                    {
                        game.Players = game.Players.OrderBy(p => p.SeatNumber).ToList();

                        if (amount <= game.Players.Where(p => p.IsPlaying).Min(p => p.Stack))
                        {
                            game.RoundRaiseAmount = amount;
                            var player = game.Players.FirstOrDefault(p => p.Name == userName);
                            if (player != null)
                            {
                                game.Pot += (amount - player.RoundBet);
                                user.Stack = (user.Stack - amount + player.RoundBet);
                                player.Stack = (player.Stack - amount + player.RoundBet);
                                player.RoundBet = amount;
                                player.HasMadeMove = true;
                                _db.Bets.Add(new Entities.Bet
                                {
                                    BetAmount = amount,
                                    Game = _db.Games.First(g => g.Id == game.Id),
                                    Player = _db.Players.First(p => p.SeatPlace == player.SeatNumber && p.GameId == game.Id),
                                    Round = (int)game.BetRound,
                                });
                                _db.SaveChanges();
                            }
                            game.CurrentPlayer = game.Players.Where(p => p.IsPlaying).
                                OrderBy(p => p.SeatNumber).
                                First(p => p.SeatNumber +
                                        (game.Players.Where(p => p.IsPlaying).Max(p => p.SeatNumber) == user.SeatNumber ? 6 : 0)
                                > user.SeatNumber).Name;

                            await SendGameInformation(user.TableId);
                        }
                    }
                }
            }
        }

        public async Task ActionFold()
        {
            var user = Users.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            if (user != null)
            {
                var game = Games.FirstOrDefault(g => g.TableId == user.TableId);
                if (game != null)
                {
                    var player = game.Players.FirstOrDefault(p => p.Name == user.Name);
                    if (player != null)
                    {
                        player.IsPlaying = false;
                        if (game.Players.Where(p => p.IsPlaying).Count() < 2)
                        {
                            await NominateWinners(game);
                            return;
                        }
                        game.CurrentPlayer = game.Players.Where(p => p.IsPlaying).
                            OrderBy(p => p.SeatNumber).
                            First(p => p.SeatNumber +
                                    (game.Players.Where(p => p.IsPlaying).Max(p => p.SeatNumber) == user.SeatNumber ? 6 : 0)
                            > user.SeatNumber).Name;
                    }
                }
                await SendGameInformation(user.TableId);
            }
        }

        public async Task GetGame(int gameId)
        {
            _db.Players.Include(p => p.User);
            var game = _db.Games.Where(g => g.Id == gameId).Include(g => g.Bets).
                                                            Include(g => g.Players).
                                                            ThenInclude(p => p.User).
                                                            Include(g => g.Board).First();
            var bets = game.Bets.OrderBy(b => b.Id);
            var gameInformation = new ReviewGameInformation();
            foreach (var player in game.Players)
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
            foreach (var bet in bets)
            {
                switch (bet.Round)
                {
                    case 1:
                        if (gameInformation.TableCards.Count == 0)
                            gameInformation.TableCards.AddRange(new List<int> { game.Board.Card1, game.Board.Card2, game.Board.Card3 }); break;
                    case 2:
                        if (gameInformation.TableCards.Count == 3)
                            gameInformation.TableCards.Add(game.Board.Card4); break;
                    case 3:
                        if (gameInformation.TableCards.Count == 4)
                            gameInformation.TableCards.Add(game.Board.Card5); break;
                }
                if (bet.BetAmount < 0)
                {
                    gameInformation.Players.FirstOrDefault(p => p.Username == bet.Player.User.UserName).IsPlaying = false;
                }
                gameInformation.CurrentPlayer = bet.Player.User.UserName;
                gameInformation.Players.Where(p => p.Username == gameInformation.CurrentPlayer).First().Bet = bet.BetAmount;
                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveGameInformation", JsonSerializer.Serialize(gameInformation));
                await Task.Delay(2000);
            }
        }
    }
}

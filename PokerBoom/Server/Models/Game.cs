using PokerBoom.Shared.Models;

namespace PokerBoom.Server.Models
{
    public class Game
    {
        public int Id { get; set; }
        public Deck Deck { get; set; }
        public List<int> TableCards { get; set; }

        public List<Player> Players { get; set; }

        public int TableId { get; set; }
        public int SmallBlind { get; set; }
        public int RoundRaiseAmount { get; set; }

        public List<string> Winners { get; set; }

        public Round BetRound { get; set; }
        public string CurrentPlayer { get; set; }
        public int Pot { get; set; }

        public Game(int tableId, int smallBlind, int smallBlindIndex)
        {
            Deck = new Deck();
            TableCards = new List<int>();
            Players = new List<Player>();
            TableId = tableId;
            SmallBlind = smallBlind;
            Winners = new List<string>();
            BetRound = Round.PreFlop;
            CurrentPlayer = "";
            RoundRaiseAmount = 0;
            Pot = 0;
        }
    }
}

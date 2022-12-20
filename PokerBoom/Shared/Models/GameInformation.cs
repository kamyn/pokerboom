using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerBoom.Shared.Models
{
    public class GameInformation
    {
        public List<Player> Players { get; set; }
        public List<Card> TableCards { get; set; }
        public List<Card> Hand { get; set; }
        public bool GameInProgress { get; set; }
        public string CurrentPlayer { get; set; }
        public int SmallBlindIndex { get; set; }
        public int BigBlindIndex { get; set; }
        public string Winner { get; set; }
        public int RaiseAmount { get; set; }
        public int PlayerRaise { get; set; }
        public GameInformation()
        {
            TableCards = new List<Card>();
            Hand = new List<Card>();
            GameInProgress = false;
            RaiseAmount = 0;
            PlayerRaise = 0;
        }
    }
}

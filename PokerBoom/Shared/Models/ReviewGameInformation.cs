using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerBoom.Shared.Models
{
    public class ReviewGameInformation
    {
        public List<ReviewGamePlayer> Players { get; set; }
        public List<int> TableCards { get; set; }
        public string CurrentPlayer { get; set; }
        public int RoundRaiseAmount { get; set; }
        public int Pot { get; set; }
        public List<string> Winners { get; set; }
        public ReviewGameInformation()
        {
            TableCards = new List<int>();
            Winners = new List<string>();
        }
    }
}

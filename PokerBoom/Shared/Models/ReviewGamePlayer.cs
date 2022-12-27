using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerBoom.Shared.Models
{
    public class ReviewGamePlayer
    {
        public string Username { get; set; }

        public List<int> HandCards { get; set; }
        public bool IsPlaying { get; set; }

        public int SeatNumber { get; set; }

        public int Stack { get; set; }

        public int Bet { get; set; }

        public ReviewGamePlayer()
        {
            IsPlaying = false;
            HandCards = new List<int>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerBoom.Shared.Models
{
    public class Player
    {
        public string Username { get; set; }

        public bool IsReady { get; set; }

        public bool IsPlaying { get; set; }

        public int SeatNumber { get; set; }

        public int GameMoney { get; set; }

        public Player()
        {
            IsReady = false;
            IsPlaying = false;
        }
    }
}

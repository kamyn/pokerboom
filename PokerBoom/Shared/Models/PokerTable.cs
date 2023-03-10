using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerBoom.Shared.Models
{
    public class PokerTable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Players { get; set; }
        public int SmallBlind { get; set; }
        public readonly int MaxPlayers = 6;
    }
}

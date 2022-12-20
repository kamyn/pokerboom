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
        public string Players { get; set; }
        public double SmallBlind { get; set; }
    }
}

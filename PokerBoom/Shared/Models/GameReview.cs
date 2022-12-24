using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerBoom.Shared.Models
{
    public class GameReview
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public int Players { get; set; }
        public int SmallBlind { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerBoom.Shared.Models
{
    public class GetGamesResultViewModel
    {
        public bool Successful { get; set; }
        public IEnumerable<GameReview> Games { get; set; }
        public string Error { get; set; }
    }
}

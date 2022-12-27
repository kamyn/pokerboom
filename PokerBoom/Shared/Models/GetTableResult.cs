using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerBoom.Shared.Models
{
    public class GetTableResult
    {
        public bool Successful { get; set; }
        public PokerTable PokerTable { get; set; }
        public string Error { get; set; }
    }
}

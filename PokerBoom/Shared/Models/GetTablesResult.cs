using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerBoom.Shared.Models
{
    public class GetTablesResult
    {
        public bool Successful { get; set; }
        public IEnumerable<PokerTable> PokerTables { get; set; }
        public string Error { get; set; }
    }
}

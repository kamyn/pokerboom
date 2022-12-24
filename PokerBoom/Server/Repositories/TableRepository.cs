using PokerBoom.Shared.Models;

namespace PokerBoom.Server.Repositories
{
    public class TableRepository : ITableRepository
    {
        private List<PokerTable> _tables = new List<PokerTable>
        {
            new PokerTable { Id = 1, Name = "стол 1", Players = 3, SmallBlind = 10 },
            new PokerTable { Id = 2, Name = "стол 2", Players = 2, SmallBlind = 50 },
            new PokerTable { Id = 3, Name = "стол 3", Players = 5, SmallBlind = 20 },
            new PokerTable { Id = 4, Name = "стол 4", Players = 0, SmallBlind = 200 }
        };

        public async Task<IEnumerable<PokerTable>> GetTables()
        {
            return _tables;
        }

        public async Task<PokerTable> GetTableById(int tableId)
        {
            return _tables.FirstOrDefault(e => e.Id == tableId);
        }
    }
}

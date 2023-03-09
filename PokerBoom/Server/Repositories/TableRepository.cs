using PokerBoom.Server.Data;
using PokerBoom.Shared.Models;

namespace PokerBoom.Server.Repositories
{
    public class TableRepository : ITableRepository
    {
        private readonly AppDbContext _db;
        public TableRepository(AppDbContext db) 
        {
            _db = db;
        }
        
        public async Task<IEnumerable<PokerTable>> GetTables()
        {
            var tables = new List<PokerTable>();
            foreach (var table in _db.Tables)
            {
                tables.Add(new PokerTable
                {
                    Id = table.Id,
                    Name = table.Name,
                    SmallBlind = table.SmallBlind,
                    Players = table.Players
                });
            }
            return tables;
        }

        public async Task<PokerTable> GetTableById(int tableId)
        {
            var table = _db.Tables.FirstOrDefault(t => t.Id == tableId);
            if (table == null)
                return new PokerTable { Id = -1, };
            return new PokerTable { 
                Id = table.Id,
                Name= table.Name,
                SmallBlind = table.SmallBlind,
                Players = table.Players
            };
        }

        public async Task SetPlayers(int tableId, int players)
        {
            var table = _db.Tables.First(t => t.Id == tableId);
            if (table != null)
            {
                table.Players = players;
                _db.SaveChanges();
            }
        }
        public async Task<bool> CreateNewTable(string name, int smallblind)
        {
            var tables = _db.Tables;
            tables.Add(new Entities.Table
            {
                Id = tables.Count() == 0 ? 1 : tables.OrderBy(t => t.Id).Last().Id + 1,
                Name = name,
                SmallBlind = smallblind,
                Players = 0
            });
            _db.SaveChanges();
            return true;
        }

        public async Task<bool> RemoveTable(int tableId)
        {
            var tables = _db.Tables;
            var table = _db.Tables.FirstOrDefault(t => t.Id == tableId);
            if (table != null)
                tables.Remove(table);
            _db.SaveChanges();
            return true;
        }
    }
}

using PokerBoom.Shared.Models;

namespace PokerBoom.Server.Repositories
{
    public interface ITableRepository
    {
        public Task<IEnumerable<PokerTable>> GetTables();

        public Task<PokerTable> GetTableById(int tableId);
    }
}

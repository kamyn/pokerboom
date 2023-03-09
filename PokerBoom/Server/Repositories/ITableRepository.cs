using PokerBoom.Shared.Models;

namespace PokerBoom.Server.Repositories
{
    public interface ITableRepository
    {
        public Task<IEnumerable<PokerTable>> GetTables();

        public Task<PokerTable> GetTableById(int tableId);

        public Task SetPlayers(int tableId, int players);

        public Task<bool> CreateNewTable(string name, int smallblind);

        public Task<bool> RemoveTable(int tableId);

    }
}

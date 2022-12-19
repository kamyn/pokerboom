using PokerBoom.Shared.Models;

namespace PokerBoom.Client.Services
{
    public interface ITableService
    {
        //Task<CreateTableResult> Create(CreateTableModel model);
        Task<GetTablesResult> GetList();
        Task<PokerTable> GetById(int id);
        //Task<DeleteTableResult> Delete(int id);
    }
}

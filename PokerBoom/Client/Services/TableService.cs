using PokerBoom.Shared.Models;
using System.Net.Http.Json;

namespace PokerBoom.Client.Services
{
    public class TableService : ITableService
    {
        private readonly HttpClient _httpClient;
        public TableService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GetTablesResult> GetList()
        {
            var result = await _httpClient.GetFromJsonAsync<GetTablesResult>("/api/table");
            return result;
        }

        public async Task<PokerTable> GetById(int id)
        {
            var result = await _httpClient.GetFromJsonAsync<PokerTable>($"/api/table/{id}");
            return result;
        }
    }
}

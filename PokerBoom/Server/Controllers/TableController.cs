using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokerBoom.Server.Data;
using PokerBoom.Shared.Models;
using PokerBoom.Server.Repositories;

namespace PokerBoom.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly AppDbContext _db;
        public TableController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<GetTablesResult>> GetTables()
        {
            var tables = new List<PokerTable>();
            foreach (var table in _db.Tables)
                tables.Add(new PokerTable { Id = table.Id, Name = table.Name, SmallBlind = table.SmallBlind });
            return Ok(new GetTablesResult { Successful = true, PokerTables = tables });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PokerTable>> GetTableById(int id)
        {
            return Ok(); // new GetTablesResult { Successful = true, PokerTables = await TableRepository.GetTableById(id) }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokerBoom.Server.Data;
using PokerBoom.Shared.Models;

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
            return Ok(new List<GetTablesResult>());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PokerTable>> GetTableById(int id)
        {
            return Ok(new PokerTable());
        }
    }
}

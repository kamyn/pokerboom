using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokerBoom.Server.Data;
using PokerBoom.Shared.Models;
using PokerBoom.Server.Repositories;
using System.Runtime.CompilerServices;

namespace PokerBoom.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ITableRepository _tableRepository;
        public TableController(AppDbContext db, ITableRepository tableRepository)
        {
            _db = db;
            _tableRepository = tableRepository;
        }

        [HttpGet]
        public async Task<ActionResult<GetTablesResult>> GetTables()
        {
            var tables = await _tableRepository.GetTables();
            return Ok(new GetTablesResult { Successful = true, PokerTables = tables });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PokerTable>> GetTableById(int id)
        {
            var table = await _tableRepository.GetTableById(id);
            return Ok(new GetTableResult { Successful = true, PokerTable = table});
        }
    }
}

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
        private readonly ITableRepository _tableRepository;
        public TableController(ITableRepository tableRepository)
        {
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

        [HttpPost]
        [Route("/api/createnewtable")]
        public async Task<IActionResult> CreateNewTable(CreateNewTableViewModel createNewTableVM)
        {
            if (createNewTableVM.Name != null)
            {
                var result = await _tableRepository.CreateNewTable(createNewTableVM.Name, createNewTableVM.SmallBlind);
                if (result)
                {
                    return Ok(new CreateNewTableResultViewModel { Success = true });
                }
                return Ok(new CreateNewTableResultViewModel { Success = false });
            }
            return Ok(new CreateNewTableResultViewModel { Success = false });
        }

        [HttpPost]
        [Route("/api/removetable")]
        public async Task<IActionResult> RemoveTable(RemoveTableViewModel removeTableVM)
        {
            var user = HttpContext.User.Identity;
            if (removeTableVM != null)
            {
                var result = await _tableRepository.RemoveTable(removeTableVM.TableId);
                if(result)
                {
                    return Ok(new RemoveTableResultViewModel { Success = true });
                }
                return Ok(new RemoveTableResultViewModel { Success = false });
            }
            return Ok(new RemoveTableResultViewModel { Success = false });
        }
    }
}

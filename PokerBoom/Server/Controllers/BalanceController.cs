using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PokerBoom.Server.Entities;
using PokerBoom.Shared.Models;

namespace PokerBoom.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BalanceController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public BalanceController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetBalance(string username)
        {
            int balance = (await _userManager.FindByNameAsync(username)).Currency;
            return Ok(new GetBalanceViewModel { Balance = balance });
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PokerBoom.Server.Entities;
using PokerBoom.Shared.Models;
using PokerBoom.Server.Repositories;

namespace PokerBoom.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BalanceController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepostitory _userRepostitory;
        public BalanceController(UserManager<ApplicationUser> userManager,
                                 IUserRepostitory userRepostitory)
        {
            _userManager = userManager;
            _userRepostitory = userRepostitory;
        }

        [HttpGet]
        public async Task<IActionResult> GetBalance(string username)
        {
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser != null) 
            {
                int balance = appUser.Currency;
                return Ok(new GetBalanceViewModel { Balance = balance });
            }
            return Ok(new GetBalanceViewModel { Balance = 0 });
        }


        [HttpPost]
        [Route("/api/changebalance")]
        public async Task<IActionResult> ChangeBalance(ChangeBalanceViewModel changeBalanceVM)
        {
            if (changeBalanceVM.UserName != null)
            {
                var result = await _userRepostitory.ChangeBalance(changeBalanceVM.UserName, changeBalanceVM.BalanceValue);
                if (result)
                {
                    return Ok(new ChangeBalanceResultViewModel { Success = true });
                }
                return Ok(new ChangeBalanceResultViewModel { Success = false });
            }
            return Ok(new ChangeBalanceResultViewModel { Success = false });
        }
    }
}

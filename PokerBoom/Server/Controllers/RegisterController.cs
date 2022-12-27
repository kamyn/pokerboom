using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PokerBoom.Server.Entities;
using PokerBoom.Shared.Models;

namespace PokerBoom.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController: ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public RegisterController(UserManager<ApplicationUser> userManager,
                               SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) 
                return BadRequest(new RegisterResultViewModel { Success = false, Error = "Введенные данные некорректны" });

            var user = await _userManager.FindByNameAsync(model.Login);
            if (user != null)
                return BadRequest(new RegisterResultViewModel { Success = false, Error = "Пользователь с таким именем уже существует" });

            var account = new ApplicationUser { UserName = model.Login, EmailConfirmed = false, Currency = 1000 };
            var result = await _userManager.CreateAsync(account, model.Password); 
            await _userManager.AddToRoleAsync(account, "User");

            return Ok(new RegisterResultViewModel { Success = true });
        }
    }
}

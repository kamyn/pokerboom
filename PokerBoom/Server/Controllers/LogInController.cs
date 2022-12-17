using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PokerBoom.Server.Data;
using PokerBoom.Server.Entities;
using PokerBoom.Shared.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PokerBoom.Server.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class LogInController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public LogInController(UserManager<ApplicationUser> userManager, 
                               SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {   
            var result = await _signInManager.PasswordSignInAsync(model.Login, model.Password, false, false);

            if (!result.Succeeded)
            {
                return Ok(new LoginResultViewModel { Success = false, Error = "No user found" });
            }

            var user = await _userManager.FindByNameAsync(model.Login);
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Login)
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthOptions.SECURITY_KEY));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddDays(Convert.ToInt32(AuthOptions.EXPIRY_IN_DAY));
            var token = new JwtSecurityToken(AuthOptions.ISSUER,
                                             AuthOptions.AUDIENCE,
                                             claims,
                                             expires: expiry,
                                             signingCredentials: credentials);
            return Ok(new LoginResultViewModel { Success = true, Token = new JwtSecurityTokenHandler().WriteToken(token)});
        }
    }
}

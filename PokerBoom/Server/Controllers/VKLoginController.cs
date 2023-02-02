using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PokerBoom.Shared.Models;
using PokerBoom.Server.Entities;
using PokerBoom.Shared;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace PokerBoom.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VKLoginController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public VKLoginController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [ActionName("vklogin")]
        public async Task<IActionResult> VKLogin(string code)
        {
            var result = await new HttpClient().GetAsync($"{AuthOptions.VK_TOKEN_END_POINT}?" +
                                                        $"client_id={AuthOptions.VK_CLIENT_ID}&" +
                                                        $"client_secret={AuthOptions.VK_CLIENT_SECRET}&" +
                                                        $"redirect_uri={Constants.VK_REDIRECT_URI}&" +
                                                        $"code={code}");

            var response = JsonSerializer.Deserialize<VKLoginResultModel>(result.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            if (response.Error != null)
                return BadRequest(new LoginResultViewModel { Error = response.ErrorDescription });

            try
            {
                var user = _userManager.Users.FirstOrDefault(x => x.VkId == response.UserId.ToString());
                if (user == null)
                {
                    var account = new ApplicationUser { UserName = response.UserId.ToString(), EmailConfirmed = false, VkId = response.UserId.ToString(), Currency = 1000 };
                    var res = await _userManager.CreateAsync(account);
                    await _userManager.AddToRoleAsync(account, "User");
                    user = _userManager.Users.FirstOrDefault(x => x.VkId == response.UserId.ToString());
                }

                await _signInManager.SignInAsync(user, true);

                var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.UserName) };
                var roles = await _userManager.GetRolesAsync(user);
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
                return Ok(new LoginResultViewModel { Success = true, Token = new JwtSecurityTokenHandler().WriteToken(token) });
            }

            catch (Exception ex)
            {

            }
            return BadRequest();
            
        }
    }
}

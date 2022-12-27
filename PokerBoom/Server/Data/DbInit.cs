using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PokerBoom.Server.Entities;
using System.Security.Claims;

namespace PokerBoom.Server.Data
{
    public class DbInit
    {
        public static async void Init(IServiceProvider provider)
        {
            var userManager = provider.GetService<UserManager<ApplicationUser>>();
            var user = new ApplicationUser { UserName = "root1", EmailConfirmed = false, Currency = 1000 }; // new ApplicationUser { UserName = "pavel1", EmailConfirmed = false, VkId = "250617147", Currency = 1000 };
            var result = userManager.CreateAsync(user, "123").GetAwaiter().GetResult(); // CreateAsync(user, pass);
            userManager.AddToRoleAsync(user, "Administrator").GetAwaiter().GetResult();
        }
    }
}

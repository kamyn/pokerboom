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
            if (userManager != null)
            {
                var root = new ApplicationUser { UserName = "root", EmailConfirmed = false, Currency = 1000 };
                var user = new ApplicationUser { UserName = "user", EmailConfirmed = false, Currency = 1000 };
                var result1 = userManager.CreateAsync(root, "123").GetAwaiter().GetResult();
                var result2 = userManager.CreateAsync(user, "123").GetAwaiter().GetResult();
                if (result1.Succeeded && result2.Succeeded)
                {
                    userManager.AddToRoleAsync(root, "Administrator").GetAwaiter().GetResult();
                    userManager.AddToRoleAsync(user, "User").GetAwaiter().GetResult();
                }
            }
        }
    }
}

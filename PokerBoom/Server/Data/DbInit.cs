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
                var user1 = new ApplicationUser { UserName = "user1", EmailConfirmed = false, Currency = 1000 }; // new ApplicationUser { UserName = "pavel1", EmailConfirmed = false, VkId = "250617147", Currency = 1000 };
                var user2 = new ApplicationUser { UserName = "user2", EmailConfirmed = false, Currency = 1000 };
                var result1 = userManager.CreateAsync(user1, "123").GetAwaiter().GetResult();
                var result2 = userManager.CreateAsync(user2, "123").GetAwaiter().GetResult();
                if (result1.Succeeded && result2.Succeeded)
                {
                    userManager.AddToRoleAsync(user1, "Administrator").GetAwaiter().GetResult();
                    userManager.AddToRoleAsync(user2, "Administrator").GetAwaiter().GetResult();
                }
            }
        }
    }
}

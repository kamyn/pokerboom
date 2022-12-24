using Microsoft.AspNetCore.Identity;

namespace PokerBoom.Server.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string VkId { get; set; }
        public int Currency { get; set; }
        public List<Player> Players { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace PokerBoom.Shared.Models
{ 
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string? Login { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}

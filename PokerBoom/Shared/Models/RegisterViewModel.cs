using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerBoom.Shared.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите логин")]
        public string? Login { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        public string? Password { get; set; }

        [Required]
        [Compare(nameof(Password))]
        public string? PasswordConfirmed { get; set; }
    }
}

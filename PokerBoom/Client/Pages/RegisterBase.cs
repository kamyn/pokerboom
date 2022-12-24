using Microsoft.AspNetCore.Components;
using PokerBoom.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace PokerBoom.Client.Pages
{
    public class RegisterBase : ComponentBase
    {
        [Inject] protected NavigationManager _navigationManager { get; set; }
        protected RegisterViewModel User { get; set; } = new RegisterViewModel();

        protected async Task Register()
        {

        }
    }
}

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace PokerBoom.Client.Shared
{
    public class NavMenuBase : ComponentBase
    {
        private bool collapseNavMenu = true;
        protected string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        public NavMenuBase()
        {

        }
        protected override async Task OnInitializedAsync()
        {
            
        }

        protected void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }
    }
}

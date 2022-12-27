using Microsoft.AspNetCore.Components;
using PokerBoom.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using static System.Net.WebRequestMethods;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PokerBoom.Client.Pages
{
    public class RegisterBase : ComponentBase
    {
        [Inject] protected NavigationManager _navigationManager { get; set; }
        [Inject] protected HttpClient _http { get; set; }
        protected RegisterViewModel User { get; set; } = new RegisterViewModel();
        protected string Error { get; set; }

        protected async Task Register()
        {
            var response = await _http.PostAsJsonAsync("/api/register", User);
            var result = await response.Content.ReadFromJsonAsync<RegisterResultViewModel>();
            if (result.Success)
                _navigationManager.NavigateTo("/");
            Error = result.Error;
            StateHasChanged();
        }
    }
}

using Microsoft.AspNetCore.Components;
using PokerBoom.Shared.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using PokerBoom.Shared;
using Microsoft.AspNetCore.Http;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace PokerBoom.Client.Pages
{
    public class LoginBase : ComponentBase
    {
        [Inject] protected HttpClient _http { get; set; }
        [Inject] protected NavigationManager _navigationManager { get; set; }
        [Inject] protected ILocalStorageService _localStorage { get; set; }
        [Inject] protected AuthenticationStateProvider _authenticationStateProvider { get; set; }

        protected LoginViewModel User { get; set; } = new LoginViewModel();
        //protected string? LocalValue { get; set; }

        //protected override async Task OnInitializedAsync()
        //{
        //    await _localStorage.SetItemAsync("authToken", "123");
        //    LocalValue = await _localStorage.GetItemAsync<string>("authToken");
        //}

        protected async Task Login()
        {
            var response = await _http.PostAsJsonAsync("/api/login", User);
            var result = await response.Content.ReadFromJsonAsync<LoginResultViewModel>();
            if (result.Success)
            {
                var token = result.Token;
                await _localStorage.SetItemAsync("authToken", result.Token);
                ((AppAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(result.Token);
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Token);
                _navigationManager.NavigateTo("/");
            }
        }

        protected async Task VKLogin()
        {
            _navigationManager.NavigateTo($"{Constants.VK_AUTH_URL}?" +
                                          $"client_id={Constants.VK_CLIENT_ID}&" +
                                          $"redirect_uri={Constants.VK_REDIRECT_URI}&" +
                                          $"display=popup&scope=friend&response_type=code&v=5.131");
        }
    }
}

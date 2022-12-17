using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace PokerBoom.Client
{
    public class AppAuthenticationStateProvider : AuthenticationStateProvider
    {
        private HttpClient _httpClient { get; set; }
        private ILocalStorageService _localStorage { get; set; }

        public AppAuthenticationStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            var jwtToken = new JwtSecurityToken(token);

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims, "jwt")));
        }

        public void MarkUserAsAuthenticated(string token)
        {
            var jwtToken = new JwtSecurityToken(token);
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims, "jwt"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public void MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }
    }
}

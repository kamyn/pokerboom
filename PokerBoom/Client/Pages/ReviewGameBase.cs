using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Blazored.LocalStorage;
using System.Text.Json;
using PokerBoom.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authorization;
using PokerBoom.Client.States;
using System.Net.Http;
using System.Net.Http.Json;


namespace PokerBoom.Client.Pages
{
    public class ReviewGameBase : ComponentBase
    {
        [Inject] protected ILocalStorageService _localStorage { get; set; }
        [Inject] protected NavigationManager _navigationManager { get; set; }
        [Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] protected HttpClient _httpClient { get; set; }
        [Inject] protected BalanceState _balanceState { get; set; }

        protected ReviewGameInformation ReviewGameInformation { get; set; }

        protected AuthenticationState AuthState { get; set; }
        private HubConnection _hubConnection;

        protected override async Task OnInitializedAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            AuthState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

            var response = await _httpClient.GetAsync($"/api/balance?username={AuthState.User.Identity.Name}");
            int balance = (await response.Content.ReadFromJsonAsync<GetBalanceViewModel>()).Balance;

            _balanceState.OnBalanceChanged.Invoke(balance);

            try
            {
                _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri("/gamehub"), options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult<string?>(token);
                    options.SkipNegotiation = true;
                    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
                })
                .Build();

                _hubConnection.On("ReceiveGameInformation", (object gameInformation) =>
                {
                    ReviewGameInformation = JsonSerializer.Deserialize<ReviewGameInformation>(gameInformation.ToString());
                    StateHasChanged();
                });
                await _hubConnection.StartAsync();

                var gameId = await _localStorage.GetItemAsync<int>("reviewGameId");
                await _hubConnection.SendAsync("GetGame", gameId);
                await base.OnInitializedAsync();
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }

            StateHasChanged();

            await base.OnInitializedAsync();
        }

        protected async Task LeaveReviewGame()
        {
            await _localStorage.RemoveItemAsync("reviewGameId");
            await _hubConnection.StopAsync();
            _navigationManager.NavigateTo("/gameslist");
        }
    }
}

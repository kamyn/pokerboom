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
    [Authorize]
    public class GameBase : ComponentBase
    {
        [Inject] protected ILocalStorageService _localStorage { get; set; }
        [Inject] protected NavigationManager _navigationManager { get; set; }
        [Inject] protected HttpClient? _httpClient { get; set; }
        [Inject] protected BalanceState _balanceState { get; set; }
        protected int RaiseBet { get; set; } = 0;

        [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        public AuthenticationState AuthState { get; set; }

        private HubConnection _hubConnection;

        public GameInformation GameInformation { get; set; }
        public List<int> HandCards { get; set; } 

        protected override async Task OnInitializedAsync()
        {
            AuthState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

            var response = await _httpClient.GetAsync($"/api/balance?username={AuthState.User.Identity.Name}");
            int balance = (await response.Content.ReadFromJsonAsync<GetBalanceViewModel>()).Balance;

            _balanceState.OnBalanceChanged.Invoke(balance);

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri("/gamehub"))
                .Build();

            _hubConnection.On("ReceiveHandCards", (object cards) =>
            {
                HandCards = JsonSerializer.Deserialize<List<int>>(cards.ToString());
                StateHasChanged();
            });

            _hubConnection.On("ReceiveGameInformation", (object gameInformation) =>
            {
                GameInformation = JsonSerializer.Deserialize<GameInformation>(gameInformation.ToString());
                StateHasChanged();
            });

            _hubConnection.On("ReceiveKick", async () =>
            {
                await _localStorage.RemoveItemAsync("currentTable");
                _navigationManager.NavigateTo("/");
            });

            await _hubConnection.StartAsync();

            var playerInfo = new KeyValuePair<int, int>(await _localStorage.GetItemAsync<int>("currentTable"), 
                await _localStorage.GetItemAsync<int>("stackAmount"));
            await _hubConnection.SendAsync("AddToUsers", playerInfo);

            await base.OnInitializedAsync();
        }

        protected async Task LeaveTable()
        {
            await _localStorage.RemoveItemAsync("currentTable");
            await _hubConnection.StopAsync();
            _navigationManager.NavigateTo("/");
        }

        protected async Task Check()
        {
            await _hubConnection.SendAsync("ActionCheck");
        }

        protected async Task Fold()
        {
            await _hubConnection.SendAsync("ActionFold");
        }

        protected async Task Call()
        {
            await _hubConnection.SendAsync("ActionCall");
        }
        protected async Task AllIn()
        {
            await _hubConnection.SendAsync("ActionAllIn");
        }

        protected async Task Raise()
        {
            await _hubConnection.SendAsync("ActionRaise", RaiseBet);
        }
    }
}

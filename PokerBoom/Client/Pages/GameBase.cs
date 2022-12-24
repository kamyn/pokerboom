using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Blazored.LocalStorage;
using System.Text.Json;
using PokerBoom.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace PokerBoom.Client.Pages
{
    [Authorize]
    public class GameBase : ComponentBase
    {
        [Inject] protected ILocalStorageService _localStorage { get; set; }
        [Inject] protected NavigationManager _navigationManager { get; set; }
        protected double RaiseBet { get; set; }

        [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        public AuthenticationState AuthState { get; set; }

        private HubConnection _hubConnection;

        public GameInformation GameInformation { get; set; }
        public List<int> HandCards { get; set; } 
        //public GameInformation GameInformation { get; set; } = new GameInformation()
        //{
        //    Players = new List<GamePlayer> { new GamePlayer() { Username = "pavel1", SeatNumber = 1 },
        //                                     new GamePlayer() { Username = "egor", SeatNumber = 2 },
        //                                    new GamePlayer() { Username = "timur", SeatNumber = 3 },
        //                                    new GamePlayer() { Username = "lexa", SeatNumber = 4 },
        //                                    new GamePlayer() { Username = "kirill", SeatNumber = 5 },
        //                                    new GamePlayer() { Username = "tasya", SeatNumber = 6 }},
        //    TableCards = new List<int> { 1, 2, 3, 4, 5}, 
        //    HandCards = new List<int> { 5, 43 },
        //    CurrentPlayer = "pavel1"
        //};

        protected override async Task OnInitializedAsync()
        {
            AuthState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

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

            var playerInfo = new KeyValuePair<int, int>(await _localStorage.GetItemAsync<int>("currentTable"), 100); // 100 is stack amount
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
            await _hubConnection.SendAsync("ActionRaise", 10); // 10 is raise amount
            //if (GameInformation.PlayerRaise > 0 &&
            //    GameInformation.Players.First(e => e.Username == AuthState.User.Identity.Name).Stack >
            //    GameInformation.PlayerRaise + GameInformation.RaiseAmount)
            //{
            //    await _hubConnection.SendAsync("ActionRaise", GameInformation.PlayerRaise);
            //}
            //GameInformation.PlayerRaise = 0;
        }
    }
}

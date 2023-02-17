using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Blazored.LocalStorage;
using System.Text.Json;
using PokerBoom.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace PokerBoom.Client.Pages
{
    public class ReviewGameBase : ComponentBase
    {
        [Inject] protected ILocalStorageService _localStorage { get; set; }
        [Inject] protected NavigationManager _navigationManager { get; set; }
        [Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        protected ReviewGameInformation ReviewGameInformation { get; set; }

        protected AuthenticationState AuthState { get; set; }
        private HubConnection _hubConnection;

        protected override async Task OnInitializedAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            AuthState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

            try
            {
                _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri("/gamereviewhub"), options =>
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
            }
            catch(Exception ex)
            {
                var message = ex.Message;
            }

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

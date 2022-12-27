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

        private HubConnection _hubConnection;
        protected ReviewGameInformation ReviewGameInformation { get; set; }
        protected override async Task OnInitializedAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri("/gamereviewhub"))
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
    }
}

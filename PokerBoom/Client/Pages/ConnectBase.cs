using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace PokerBoom.Client.Pages
{
    public class ConnectBase : ComponentBase
    {
        [Inject] 
        public NavigationManager Navigation { get; set; }

        private HubConnection? hubConnection;
        protected string? messageInput;
        protected string? messageOutput;

        protected override async Task OnInitializedAsync()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(Navigation.ToAbsoluteUri("/sendallhub"))
                .Build();

            hubConnection.On<string>("ReceiveMessage", message =>
            {
                messageOutput = message;
                StateHasChanged();
            });

            await hubConnection.StartAsync();
        }

        protected async Task SendAll()
        {
            if (hubConnection is not null)
            {
                await hubConnection.SendAsync("SendMessage", messageInput);
            }
        }
    }
}

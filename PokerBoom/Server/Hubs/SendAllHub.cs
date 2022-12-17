using Microsoft.AspNetCore.SignalR;

namespace PokerBoom.Server.Hubs
{
    public class SendAllHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }

}

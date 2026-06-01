using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace RaceMS.WebMVCApp.NguyenDNT.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        // Called by client: connection.invoke("SendMessage", message)
        public async Task SendMessage(string message)
        {
            var displayName = Context.User?.FindFirst(ClaimTypes.GivenName)?.Value
                           ?? Context.User?.Identity?.Name
                           ?? "Unknown";

            var timestamp = DateTime.Now.ToString("HH:mm:ss");

            await Clients.All.SendAsync("ReceiveMessage", displayName, message, timestamp);
        }

        public override async Task OnConnectedAsync()
        {
            var displayName = Context.User?.FindFirst(ClaimTypes.GivenName)?.Value
                           ?? Context.User?.Identity?.Name
                           ?? "Unknown";

            await Clients.Others.SendAsync("UserConnected", displayName);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var displayName = Context.User?.FindFirst(ClaimTypes.GivenName)?.Value
                           ?? Context.User?.Identity?.Name
                           ?? "Unknown";

            await Clients.Others.SendAsync("UserDisconnected", displayName);
            await base.OnDisconnectedAsync(exception);
        }
    }
}

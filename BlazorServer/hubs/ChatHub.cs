using Microsoft.AspNetCore.SignalR;

namespace BlazorServerSignalRApp.Server.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            Console.WriteLine($"{user}:{message}");
            await Clients.All.SendAsync("MessageReceived", user, message);
        }
    }
}
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace WebProjekt.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            // Nachricht an alle Clients senden
            await Clients.All.SendAsync("ReceiveMessage", user, message);

            // Optional: Bot-Antwort
            if (message.ToLower().Contains("hallo"))
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "Bot", "Hallo! Wie kann ich helfen?");
            }
        }
    }
}

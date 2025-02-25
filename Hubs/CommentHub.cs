using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CampManager.Hubs
{
    public class CommentHub : Hub
    {
        public async Task SendComment(int eventId, string username, string message)
        {
            await Clients.All.SendAsync("ReceiveComment", eventId, username, message, System.DateTime.UtcNow);
        }

        public async Task JoinEventGroup(int eventId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"event_{eventId}");
        }

        public async Task LeaveEventGroup(int eventId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"event_{eventId}");
        }
    }
}

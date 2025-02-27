using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;
using System.Linq;
using CampManager.Repositories;
using System.Security.Claims;

namespace CampManager.Hubs
{
    [Authorize]
    public class CommentHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly ICounselorRepository _counselorRepository;

        public CommentHub(ApplicationDbContext context, IUserRepository userRepository, ICounselorRepository counselorRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _counselorRepository = counselorRepository;
        }

        public async Task SendComment(int eventId, string message)
        {
            var userIdClaim = Context.User?.Claims.FirstOrDefault(c => c.Type == "sub")
                ?? Context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new HubException("Пользователь не авторизован");
            }
            int userId = int.Parse(userIdClaim.Value);

            var user = await _userRepository.GetUserByIdAsync(userId);
            string displayName;
            if (user.Role == "Counselor")
            {
                var counselor = await _counselorRepository.GetCounselorByUserIdAsync(userId);
                displayName = (counselor != null) ? $"{counselor.Surname} {counselor.Name}" : user.Username;
            }
            else
            {
                displayName = user.Username;
            }

            var comment = new Comment
            {
                Event_Id = eventId,
                Message = message,
                User_Id = userId,
                CreatedAt = DateTime.UtcNow
            };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            await Clients.Group($"event_{eventId}")
                .SendAsync("ReceiveComment", eventId, displayName, message, comment.CreatedAt);
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

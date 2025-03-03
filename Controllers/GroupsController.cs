using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using CampManager.Repositories;

namespace CampManager.Controllers
{
    [Route("api/groups")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupRepository _groupRepository;
        private readonly ApplicationDbContext _context;

        public GroupsController(IGroupRepository groupRepository, ApplicationDbContext context)
        {
            _groupRepository = groupRepository;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetGroups()
        {
            var groups = await _groupRepository.GetAllGroupsAsync();
            var result = groups.Select(g => new
            {
                g.Group_Id,
                g.Name,
                g.Number,
                g.SessionCounselor_Id,
                g.SessionId,
                Session = g.Session != null ? new
                {
                    g.Session.Session_Id,
                    g.Session.Number,
                    g.Session.Type,
                    g.Session.Year,
                    g.Session.Season
                } : null,
                Counselor = g.SessionCounselor?.Counselor != null ? new
                {
                    g.SessionCounselor.Counselor.Counselor_Id,
                    g.SessionCounselor.Counselor.Name,
                    g.SessionCounselor.Counselor.Surname,
                    g.SessionCounselor.Counselor.Patronymic
                } : null
            });
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDTO request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Некорректные данные" });
            }

            // Ищем запись SessionCounselor по SessionId и CounselorId (из DTO)
            var sessionCounselor = await _context.SessionCounselors
                .FirstOrDefaultAsync(sc => sc.SessionId == request.SessionId && sc.CounselorId == request.CounselorId);
            if (sessionCounselor == null)
            {
                // Если такой записи нет, создаём новую
                sessionCounselor = new SessionCounselor
                {
                    SessionId = request.SessionId,
                    CounselorId = request.CounselorId
                };
                _context.SessionCounselors.Add(sessionCounselor);
                await _context.SaveChangesAsync();
            }

            var group = new Group
            {
                Name = request.Name,
                Number = request.Number,
                SessionCounselor_Id = sessionCounselor.SessionCounselor_Id,
                SessionId = request.SessionId
            };

            await _groupRepository.AddGroupAsync(group);
            await _groupRepository.SaveChangesAsync();

            return Ok(new { message = "Отряд создан успешно", groupId = group.Group_Id });
        }
    }
}

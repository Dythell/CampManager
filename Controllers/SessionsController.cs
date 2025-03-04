using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CampManager.Controllers
{
    [Route("api/sessions")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly ApplicationDbContext _context;

        public SessionsController(ISessionRepository sessionRepository, ApplicationDbContext context)
        {
            _sessionRepository = sessionRepository;
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateSession([FromBody] CreateSessionDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Type) || string.IsNullOrWhiteSpace(request.Season))
            {
                return BadRequest(new { message = "Тип смены и сезон обязательны" });
            }

            try
            {
                var newSession = new Session
                {
                    Number = request.Number,
                    Type = request.Type,
                    Year = request.Year,
                    Season = request.Season
                };

                await _sessionRepository.AddSessionAsync(newSession);
                await _sessionRepository.SaveChangesAsync();

                return Ok(new { message = "Смена создана успешно", sessionId = newSession.Session_Id });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка сервера при создании смены", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSessions()
        {
            try
            {
                var sessions = await _sessionRepository.GetAllSessionsAsync();
                return Ok(sessions);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка сервера при получении смен", error = ex.Message });
            }
        }

        [HttpGet("{sessionId}/details")]
        public async Task<IActionResult> GetSessionDetails(int sessionId)
        {
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Session_Id == sessionId);
            if (session == null)
                return NotFound(new { message = "Смена не найдена" });

            var events = await _context.Events
                .Where(e => e.SessionId == sessionId)
                .Select(e => new {
                    e.Event_Id,
                    e.CustomName,
                    e.Type,
                    e.DateTime,
                    e.Status
                })
                .ToListAsync();

            var groups = await _context.Groups
                .Include(g => g.SessionCounselor)
                    .ThenInclude(sc => sc.Counselor)
                .Where(g => g.SessionId == sessionId)
                .ToListAsync();

            var groupsWithChildren = groups.Select(g => new {
                GroupId = g.Group_Id,
                g.Name,
                g.Number,
                Counselor = g.SessionCounselor?.Counselor != null ? new
                {
                    g.SessionCounselor.Counselor.Counselor_Id,
                    g.SessionCounselor.Counselor.Name,
                    g.SessionCounselor.Counselor.Surname,
                    g.SessionCounselor.Counselor.Patronymic
                } : null,
                Children = _context.Children
                    .Where(c => c.GroupId == g.Group_Id)
                    .Select(c => new {
                        c.Child_Id,
                        c.Surname,
                        c.Name,
                        c.Patronymic,
                        c.BirthYear,
                        c.ParentNumber
                    })
                    .ToList()
            }).ToList();

            var result = new
            {
                Session = new
                {
                    session.Session_Id,
                    session.Number,
                    session.Type,
                    session.Year,
                    session.Season
                },
                Events = events,
                Groups = groupsWithChildren
            };

            return Ok(result);
        }

    }
}

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

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,GAdmin")]
        public async Task<IActionResult> PutGroup(int id, [FromBody] UpdateGroupDTO dto)
        {
            var existing = await _groupRepository.GetGroupByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "Отряд не найден" });

            bool isModified = false;

            if (dto.Name != null && dto.Name != existing.Name)
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return BadRequest(new { message = "Название не может быть пустым" });

                existing.Name = dto.Name;
                isModified = true;
            }

            if (dto.Number.HasValue && dto.Number.Value != existing.Number)
            {
                existing.Number = dto.Number.Value;
                isModified = true;
            }

            var newSessionId = dto.SessionId ?? existing.SessionId;
            var newCounselorId = dto.CounselorId ?? existing.SessionCounselor.CounselorId;
            if (newSessionId != existing.SessionId || newCounselorId != existing.SessionCounselor.CounselorId)
            {
                var session = await _context.Sessions.FindAsync(newSessionId);
                if (session == null)
                    return BadRequest(new { message = $"Смена с ID={newSessionId} не найдена" });

                var counselor = await _context.Counselors.FindAsync(newCounselorId);
                if (counselor == null)
                    return BadRequest(new { message = $"Вожатый с ID={newCounselorId} не найден" });

                // Находим ор создаём связь в табл SessionCounselor
                var sc = await _context.SessionCounselors
                    .FirstOrDefaultAsync(x => x.SessionId == newSessionId && x.CounselorId == newCounselorId);
                if (sc == null)
                {
                    sc = new SessionCounselor { SessionId = newSessionId, CounselorId = newCounselorId };
                    _context.SessionCounselors.Add(sc);
                    await _context.SaveChangesAsync();
                }

                existing.SessionId = newSessionId;
                existing.SessionCounselor_Id = sc.SessionCounselor_Id;
                isModified = true;
            }

            if (!isModified)
                return BadRequest(new { message = "Нет изменений для сохранения" });

            try
            {
                await _groupRepository.UpdateGroupAsync(existing);
                await _groupRepository.SaveChangesAsync();
                return Ok(new { message = "Отряд успешно обновлён", group = existing });
            }
            catch
            {
                return StatusCode(500, new { message = "Ошибка при сохранении изменений" });
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,GAdmin")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var existing = await _groupRepository.GetGroupByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "Отряд не найден" });

            try
            {
                await _groupRepository.DeleteGroupAsync(existing);
                await _groupRepository.SaveChangesAsync();
                return Ok(new { message = "Отряд успешно удалён" });
            }
            catch
            {
                return StatusCode(500, new { message = "Ошибка при удалении отряда" });
            }
        }
    }
}

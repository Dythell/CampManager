using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CampManager.Controllers
{
    [Route("api/children")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ChildrenController : ControllerBase
    {
        private readonly IChildRepository _childRepository;
        private readonly ISessionChildRepository _sessionChildRepository;
        private readonly ApplicationDbContext _context;

        public ChildrenController(IChildRepository childRepository,
                                  ISessionChildRepository sessionChildRepository,
                                  ApplicationDbContext context)
        {
            _childRepository = childRepository;
            _sessionChildRepository = sessionChildRepository;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateChild([FromBody] CreateChildDTO request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Некорректные данные" });
            }

            var child = new Child
            {
                Surname = request.Surname,
                Name = request.Name,
                Patronymic = request.Patronymic,
                BirthYear = request.BirthYear,
                ParentNumber = request.ParentNumber,
                GroupId = request.GroupId
            };

            await _childRepository.AddChildAsync(child);
            await _childRepository.SaveChangesAsync();

            // Находим группу по GroupId чтобы получить SessionId
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.Group_Id == request.GroupId);
            if (group == null)
            {
                return BadRequest(new { message = "Группа не найдена" });
            }

            // Создаем запись в SessionChild для того что бы связать ребенка со сменой
            var sessionChild = new SessionChild
            {
                SessionId = group.SessionId,
                ChildId = child.Child_Id
            };

            await _sessionChildRepository.AddSessionChildAsync(sessionChild);
            child.BirthYear = DateTime.SpecifyKind(child.BirthYear, DateTimeKind.Utc);
            await _sessionChildRepository.SaveChangesAsync();

            return Ok(new { message = "Ребенок успешно добавлен в отряд и привязан к смене", childId = child.Child_Id });
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,GAdmin")]
        public async Task<IActionResult> UpdateChild(int id, [FromBody] UpdateChildDTO dto)
        {
            var existing = await _childRepository.GetChildByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "Ребёнок не найден" });

            bool isModified = false;

            if (dto.Name != null && dto.Name != existing.Name)
            {
                existing.Name = dto.Name;
                isModified = true;
            }

            if (dto.Surname != null && dto.Surname != existing.Surname)
            {
                existing.Surname = dto.Surname;
                isModified = true;
            }

            if (dto.Patronymic != null && dto.Patronymic != existing.Patronymic)
            {
                existing.Patronymic = dto.Patronymic;
                isModified = true;
            }

            if (dto.BirthYear.HasValue && dto.BirthYear.Value != existing.BirthYear)
            {
                existing.BirthYear = dto.BirthYear.Value;
                isModified = true;
            }

            if (dto.GroupId.HasValue && dto.GroupId.Value != existing.GroupId)
            {
                existing.GroupId = dto.GroupId.Value;
                isModified = true;
            }

            if (!isModified)
                return BadRequest(new { message = "Нет изменений для сохранения" });

            await _childRepository.UpdateChildAsync(existing);
            await _childRepository.SaveChangesAsync();

            return Ok(new { message = "Ребёнок успешно обновлён", child = existing });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,GAdmin")]
        public async Task<IActionResult> DeleteChild(int id)
        {
            var existing = await _childRepository.GetChildByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "Ребёнок не найден" });

            await _childRepository.DeleteChildAsync(existing);
            await _childRepository.SaveChangesAsync();

            return Ok(new { message = "Ребёнок успешно удалён" });
        }
    }
}

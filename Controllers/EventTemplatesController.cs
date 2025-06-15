using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace CampManager.Controllers
{
    [Route("api/eventtemplates")]
    [ApiController]
    public class EventTemplatesController : ControllerBase
    {
        private readonly IEventTemplateRepository _eventTemplateRepository;

        public EventTemplatesController(IEventTemplateRepository eventTemplateRepository)
        {
            _eventTemplateRepository = eventTemplateRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetTemplates()
        {
            var templates = await _eventTemplateRepository.GetAllTemplatesAsync();
            return Ok(templates);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTemplate([FromBody] CreateEventTemplateDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Type))
            {
                return BadRequest(new { message = "Название и тип мероприятия обязательны" });
            }

            var newTemplate = new EventTemplate
            {
                Name = request.Name,
                Type = request.Type,
                DefaultDescription = request.DefaultDescription
            };

            await _eventTemplateRepository.AddTemplateAsync(newTemplate);
            await _eventTemplateRepository.SaveChangesAsync();

            return Ok(new { message = "Шаблон мероприятия создан успешно" });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,GAdmin")]
        public async Task<IActionResult> PutTemplate(int id, [FromBody] UpdateEventTemplateDTO dto)
        {
            var existing = await _eventTemplateRepository.GetTemplateByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "Шаблон мероприятия не найден" });

            bool isModified = false;

            if (dto.Name != null && dto.Name != existing.Name)
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return BadRequest(new { message = "Название не может быть пустым" });

                existing.Name = dto.Name;
                isModified = true;
            }

            if (dto.Type != null && dto.Type != existing.Type)
            {
                if (string.IsNullOrWhiteSpace(dto.Type))
                    return BadRequest(new { message = "Тип не может быть пустым" });

                existing.Type = dto.Type;
                isModified = true;
            }

            if (dto.DefaultDescription != null && dto.DefaultDescription != existing.DefaultDescription)
            {
                existing.DefaultDescription = dto.DefaultDescription;
                isModified = true;
            }

            if (!isModified)
                return BadRequest(new { message = "Нет изменений для сохранения" });

            try
            {
                await _eventTemplateRepository.UpdateTemplateAsync(existing);
                await _eventTemplateRepository.SaveChangesAsync();
                return Ok(new { message = "Шаблон мероприятия обновлён", template = existing });
            }
            catch
            {
                return StatusCode(500, new { message = "Ошибка при сохранении изменений" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,GAdmin")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            var existing = await _eventTemplateRepository.GetTemplateByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "Шаблон мероприятия не найден" });

            try
            {
                await _eventTemplateRepository.DeleteTemplateAsync(existing);
                await _eventTemplateRepository.SaveChangesAsync();
                return Ok(new { message = "Шаблон мероприятия удалён" });
            }
            catch
            {
                return StatusCode(500, new { message = "Ошибка при удалении шаблона мероприятия" });
            }
        }
    }
}

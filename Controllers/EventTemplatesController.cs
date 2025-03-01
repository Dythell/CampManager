using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;

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


    }
}

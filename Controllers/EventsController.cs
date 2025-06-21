using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using CampManager.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CampManager.Controllers
{
    [Route("api/events")]
    [ApiController]
    [Authorize(Roles = "Admin,Counselor")]
    public class EventsController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly ICounselorRepository _counselorRepository;
        private readonly ApplicationDbContext _context;

        public EventsController(
            IEventRepository eventRepository,
            ICounselorRepository counselorRepository,
            ApplicationDbContext context)
        {
            _eventRepository = eventRepository;
            _counselorRepository = counselorRepository;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDTO request)
        {
            try
            {
                var session = await _context.Sessions.FindAsync(request.SessionId);
                if (session == null)
                {
                    return BadRequest(new { message = "Смена с указанным ID не найдена" });
                }

                var counselor = await _counselorRepository.GetCounselorByUserIdAsync(request.CounselorId);
                if (counselor == null)
                {
                    return BadRequest(new { message = "Указанный ответственный вожатый не найден" });
                }

                string eventType = request.Type;
                bool isCustomEvent = request.IsCustomEvent;
                string? customName = request.CustomName;

                if (request.EventTemplateId.HasValue)
                {
                    var template = await _context.EventTemplates.FindAsync(request.EventTemplateId.Value);
                    if (template == null)
                    {
                        return BadRequest(new { message = "Шаблон мероприятия не найден" });
                    }

                    eventType = template.Type;
                    isCustomEvent = false;
                    customName = $"({template.Name})"; // Добавляем в CustomName название шаблона в скобках заместо названия самого
                }

                var newEvent = new Event
                {
                    SessionId = request.SessionId,
                    EventTemplateId = request.EventTemplateId,
                    CustomName = customName,
                    IsCustomEvent = isCustomEvent,
                    Type = eventType,
                    DateTime = request.DateTime,
                    Status = request.Status,
                    CounselorId = request.CounselorId
                };

                await _eventRepository.AddEventAsync(newEvent);
                await _eventRepository.SaveChangesAsync();

                return Ok(new { message = "Мероприятие создано успешно" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка сервера при создании мероприятия", error = ex.Message });
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            try
            {
                var events = await _context.Events
       .Include(e => e.Counselor)
       .Include(e => e.Session)
       .Include(e => e.EventTemplate)
       .Select(e => new
       {
           e.Event_Id,
           e.SessionId,
           EventName = e.IsCustomEvent ? e.CustomName : (e.EventTemplate != null ? e.EventTemplate.Name : "Без названия"), // Проверяем на null
           e.Type,
           e.DateTime,
           e.Status,
           Counselor = new
           {
               e.Counselor.Counselor_Id,
               e.Counselor.Name,
               e.Counselor.Surname
           }
       })
       .ToListAsync();


                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка сервера при получении списка мероприятий", error = ex.Message });
            }
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventById(int eventId)
        {
            try
            {
                var eventItem = await _context.Events
                    .Include(e => e.EventTemplate)
                    .Include(e => e.Counselor)
                    .Where(e => e.Event_Id == eventId)
                    .Select(e => new
                    {
                        e.Event_Id,
                        e.SessionId,
                        EventName = e.IsCustomEvent ? e.CustomName : e.EventTemplate.Name,
                        e.Type,
                        e.DateTime,
                        e.Status,
                        e.CounselorId,
                        Counselor = e.Counselor == null
                            ? null
                            : new
                            {
                                e.Counselor.Counselor_Id,
                                e.Counselor.Surname,
                                e.Counselor.Name,
                                e.Counselor.Patronymic
                            }
                    })
                    .FirstOrDefaultAsync();

                if (eventItem == null)
                    return NotFound(new { message = "Мероприятие не найдено" });

                return Ok(eventItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка сервера при получении мероприятия", error = ex.Message });
            }
        }


        [HttpPut("{eventId}")]
        [Authorize(Roles = "Admin,GAdmin")]
        public async Task<IActionResult> PutEvent(int eventId, [FromBody] UpdateEventDTO dto)
        {
            // Находим существующее мероприятие
            var existing = await _context.Events.FindAsync(eventId);
            if (existing == null)
                return NotFound(new { message = "Мероприятие не найдено" });

            bool isModified = false;

            // Проверка смены
            if (dto.SessionId.HasValue && dto.SessionId.Value != existing.SessionId)
            {
                var session = await _context.Sessions.FindAsync(dto.SessionId.Value);
                if (session == null)
                    return BadRequest(new { message = "Смена с указанным ID не найдена" });

                existing.SessionId = dto.SessionId.Value;
                isModified = true;
            }

            if (dto.CounselorId.HasValue && dto.CounselorId.Value != existing.CounselorId)
            {
                var counselor = await _counselorRepository.GetCounselorByUserIdAsync(dto.CounselorId.Value);
                if (counselor == null)
                    return BadRequest(new { message = "Указанный вожатый не найден" });

                existing.CounselorId = dto.CounselorId.Value;
                isModified = true;
            }

            if (dto.EventTemplateId.HasValue && dto.EventTemplateId.Value != existing.EventTemplateId)
            {
                existing.EventTemplateId = dto.EventTemplateId;
                isModified = true;
            }

            if (dto.IsCustomEvent.HasValue && dto.IsCustomEvent.Value != existing.IsCustomEvent)
            {
                existing.IsCustomEvent = dto.IsCustomEvent.Value;
                isModified = true;
            }

            if (dto.CustomName != null && dto.CustomName != existing.CustomName)
            {
                existing.CustomName = dto.CustomName;
                isModified = true;
            }

            if (dto.Type != null && dto.Type != existing.Type)
            {
                existing.Type = dto.Type;
                isModified = true;
            }

            if (dto.DateTime.HasValue && dto.DateTime.Value != existing.DateTime)
            {
                existing.DateTime = dto.DateTime.Value;
                isModified = true;
            }

            if (dto.Status != null && dto.Status != existing.Status)
            {
                existing.Status = dto.Status;
                isModified = true;
            }

            if (!isModified)
            {
                return Ok(new
                {
                    message = "Изменений не было. Текущее состояние мероприятия:",
                    existing
                });
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Мероприятие обновлено", existing });
            }
            catch (DbUpdateException dbEx)
            {
                return BadRequest(new { message = "Ошибка при сохранении изменений", error = dbEx.Message });
            }
        }


        [HttpDelete("{eventId}")]
        [Authorize(Roles = "Admin,GAdmin")]
        public async Task<IActionResult> DeleteEvent(int eventId)
        {
            try
            {
                var existing = await _context.Events.FindAsync(eventId);
                if (existing == null)
                    return NotFound(new { message = "Мероприятие не найдено" });

                _context.Events.Remove(existing);
                await _eventRepository.SaveChangesAsync();

                return Ok(new { message = "Мероприятие удалено" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка сервера при удалении мероприятия", error = ex.Message });
            }
        }
    }
}

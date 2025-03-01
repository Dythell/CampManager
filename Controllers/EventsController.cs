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
        [Authorize(Roles = "Counselor")]
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
        [Authorize(Roles = "Counselor")]
        public async Task<IActionResult> GetEventById(int eventId)
        {
            try
            {
                var eventItem = await _context.Events
                    .Include(e => e.EventTemplate)
                    .Where(e => e.Event_Id == eventId)
                    .Select(e => new
                    {
                        e.Event_Id,
                        e.SessionId,
                        EventName = e.IsCustomEvent ? e.CustomName : e.EventTemplate.Name,
                        e.Type,
                        e.DateTime,
                        e.Status
                    })
                    .FirstOrDefaultAsync();

                if (eventItem == null)
                {
                    return NotFound(new { message = "Мероприятие не найдено" });
                }

                return Ok(eventItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка сервера при получении мероприятия", error = ex.Message });
            }
        }

    }
}

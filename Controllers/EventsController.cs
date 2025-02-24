using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using CampManager.Repositories;

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

                var newEvent = new Event
                {
                    SessionId = request.SessionId,
                    EventTemplateId = request.EventTemplateId,
                    CustomName = request.CustomName,
                    IsCustomEvent = request.IsCustomEvent,
                    Type = request.Type,
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
                var events = await _eventRepository.GetAllEventsAsync();
                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка сервера при получении списка мероприятий", error = ex.Message });
            }
        }
    }
}

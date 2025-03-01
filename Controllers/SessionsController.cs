using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CampManager.Controllers
{
    [Route("api/sessions")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SessionsController : ControllerBase
    {
        private readonly ISessionRepository _sessionRepository;

        public SessionsController(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка сервера при получении смен", error = ex.Message });
            }
        }
    }
}

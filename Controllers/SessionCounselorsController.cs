using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using CampManager.Repositories;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;


namespace CampManager.Controllers
{
    [Route("api/sessioncounselors")]
    [ApiController]
    [Authorize]
    public class SessionCounselorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICounselorRepository _counselorRepository;

        public SessionCounselorsController(
            ApplicationDbContext context,
            ICounselorRepository counselorRepository)
        {
            _context = context;
            _counselorRepository = counselorRepository;
        }

        [HttpGet("mine")]
        [Authorize(Roles = "Counselor,Admin,GAdmin")]
        public async Task<IActionResult> GetMySessionIds()
        {
            // получаем userId из токена
            var userIdClaim = User.Claims.FirstOrDefault(c =>
                    c.Type == ClaimTypes.NameIdentifier || c.Type == "sub");
            if (userIdClaim == null)
                return Forbid();

            if (!int.TryParse(userIdClaim.Value, out var userId))
                return Forbid();

            // ищем запись Counselor по userId
            var counselor = await _counselorRepository.GetCounselorByUserIdAsync(userId);
            if (counselor == null)
                return NotFound(new { message = "Вожатый не найден" });

            // выбираем все SessionId из SessionCounselors
            var sessionIds = await _context.SessionCounselors
                .Where(sc => sc.CounselorId == counselor.Counselor_Id)
                .Select(sc => sc.SessionId)
                .ToListAsync();

            return Ok(sessionIds);
        }
    }
}

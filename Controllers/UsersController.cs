using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CampManager.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CampManager.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly ApplicationDbContext _context;

        public UsersController(IUserRepository userRepo, ApplicationDbContext context)
        {
            _userRepo = userRepo;
            _context = context;
        }

        [HttpGet("pending-admins")]
        [Authorize(Roles = "Admin,GAdmin")]
        public async Task<IActionResult> GetPendingAdmins()
        {
            var list = await _context.Users
                .Where(u => (u.Role == "Admin" || u.Role == "GAdmin") && !u.IsConfirmed)
                .Select(u => new { u.User_Id, u.Username, u.Role })
                .ToListAsync();

            return Ok(list);
        }

        [HttpPut("{id}/confirm-admin")]
        [Authorize(Roles = "Admin,GAdmin")]
        public async Task<IActionResult> ConfirmAdmin(int id)
        {
            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "Пользователь не найден" });

            if (user.Role != "Admin" && user.Role != "GAdmin")
                return BadRequest(new { message = "Это не администратор" });

            if (user.IsConfirmed)
                return BadRequest(new { message = "Администратор уже подтверждён" });

            user.IsConfirmed = true;
            await _userRepo.UpdateUserAsync(user);
            await _userRepo.SaveChangesAsync();

            return Ok(new { message = "Администратор подтверждён" });
        }

        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateMyUsername([FromBody] UpdateUserDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username))
                return BadRequest(new { message = "Имя пользователя не может быть пустым" });

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "Пользователь не найден" });

            var exists = await _context.Users
                .AnyAsync(u => u.Username == dto.Username && u.User_Id != userId);
            if (exists)
                return BadRequest(new { message = "Такой логин уже занят" });

            if (dto.Username == user.Username)
                return BadRequest(new { message = "Нет изменений для сохранения" });

            user.Username = dto.Username;
            await _userRepo.UpdateUserAsync(user);
            await _userRepo.SaveChangesAsync();

            return Ok(new { message = "Имя пользователя обновлено", username = user.Username });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,GAdmin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username))
                return BadRequest(new { message = "Имя пользователя не может быть пустым" });

            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "Пользователь не найден" });

            var exists = await _context.Users
                .AnyAsync(u => u.Username == dto.Username && u.User_Id != id);
            if (exists)
                return BadRequest(new { message = "Такой логин уже занят" });

            if (dto.Username == user.Username)
                return BadRequest(new { message = "Нет изменений для сохранения" });

            user.Username = dto.Username;
            await _userRepo.UpdateUserAsync(user);
            await _userRepo.SaveChangesAsync();

            return Ok(new { message = "Имя пользователя обновлено", user = new { user.User_Id, user.Username } });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,GAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "Пользователь не найден" });

            var comments = _context.Comments.Where(c => c.User_Id == id);
            _context.Comments.RemoveRange(comments);

            if (user.Role == "Counselor")
            {
                await _context.Events
                    .Where(e => e.CounselorId == id)
                    .ForEachAsync(e => e.CounselorId = null);

                await _context.SessionCounselors
                    .Where(sc => sc.CounselorId == id)
                    .ForEachAsync(sc => sc.CounselorId = null);

                await _context.SaveChangesAsync();
            }

            await _userRepo.DeleteUserAsync(user);
            await _userRepo.SaveChangesAsync();

            return Ok(new { message = "Пользователь и все его данные удалены" });
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using CampManager.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateMyUsername([FromBody] UpdateUserDTO dto)
        {
            if (dto.Username == null || string.IsNullOrWhiteSpace(dto.Username))
                return BadRequest(new { message = "Имя пользователя не может быть пустым" });

            // получаем свой id из токена
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
            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "Пользователь не найден" });

            if (dto.Username == null || string.IsNullOrWhiteSpace(dto.Username))
                return BadRequest(new { message = "Имя пользователя не может быть пустым" });

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

            // не тестил ⬇⬇⬇
            var userComments = _context.Comments.Where(c => c.User_Id == id);
            _context.Comments.RemoveRange(userComments);

            // если вожатый то нуллим в полях
            if (user.Role == "Counselor")
            {
                // в мероприяти
                var events = _context.Events.Where(e => e.CounselorId == id);
                await events.ForEachAsync(e => e.CounselorId = null);

                // и в сущности вожатого смены
                var scs = _context.SessionCounselors.Where(sc => sc.CounselorId == id);
                await scs.ForEachAsync(sc => sc.CounselorId = null);

                await _context.SaveChangesAsync();
            }

            await _userRepo.DeleteUserAsync(user);
            await _userRepo.SaveChangesAsync();

            return Ok(new { message = "Пользователь и все его комментарии удалены" });
        }
    }
}
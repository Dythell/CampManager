using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CampManager.Controllers
{
    [Route("api/comments")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetComments([FromQuery] int event_Id)
        {
            var comments = await _context.Comments
                .Where(c => c.Event_Id == event_Id)
                .Include(c => c.User)
                    .ThenInclude(u => u.Counselor)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            var result = comments.Select(c => new {
                c.Comment_Id,
                c.Event_Id,
                c.Message,
                CreatedAt = c.CreatedAt,
                DisplayName = c.User.Role == "Counselor" && c.User.Counselor != null
                    ? $"{c.User.Counselor.Surname} {c.User.Counselor.Name}"
                    : c.User.Username
            });

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] UpdateCommentDTO dto)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
                return NotFound(new { message = "Комментарий не найден" });

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (comment.User_Id != userId)
                return Forbid(); // тока автор может редачить

            if (string.IsNullOrWhiteSpace(dto.Message))
                return BadRequest(new { message = "Текст комментария не может быть пустым" });

            if (dto.Message == comment.Message)
                return BadRequest(new { message = "Нет изменений для сохранения" });

            comment.Message = dto.Message;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Комментарий обновлён" });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
                return NotFound(new { message = "Комментарий не найден" });

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var isOwner = comment.User_Id == userId;
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("GAdmin");

            if (!isOwner && !isAdmin)
                return Forbid();

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Комментарий удалён" });
        }


    }
}
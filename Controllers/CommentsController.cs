using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            var result = comments.Select(c => new {
                c.Comment_Id,
                c.Event_Id,
                c.Message,
                CreatedAt = c.CreatedAt,
                Username = c.User.Username
            });

            return Ok(result);
        }
    }
}

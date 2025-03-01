using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CampManager.Controllers
{
    [Route("api/eventtemplates")]
    [ApiController]
    public class EventTemplatesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public EventTemplatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTemplates()
        {
            var templates = await _context.EventTemplates.ToListAsync();
            return Ok(templates);
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CampManager.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;
        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddEventAsync(Event ev)
        {
            await _context.Events.AddAsync(ev);
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await _context.Events
                .Include(e => e.Counselor)
                .Include(e => e.Session)
                .ToListAsync();
        }

        public async Task UpdateEventAsync(Event ev)
        {
            _context.Events.Update(ev);
        }

        public async Task DeleteEventAsync(Event ev)
        {
            _context.Events.Remove(ev);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

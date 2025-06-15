using System.Collections.Generic;
using System.Threading.Tasks;

namespace CampManager.Repositories
{
    public interface IEventRepository
    {
        Task AddEventAsync(Event ev);
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task UpdateEventAsync(Event ev);
        Task DeleteEventAsync(Event ev);
        Task SaveChangesAsync();
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;

public interface ISessionRepository
{
    Task<List<Session>> GetAllSessionsAsync();
    Task<Session?> GetSessionByIdAsync(int id);
    Task AddSessionAsync(Session session);
    Task UpdateSessionAsync(Session session);
    Task DeleteSessionAsync(Session session);
    Task SaveChangesAsync();
}

using System.Collections.Generic;
using System.Threading.Tasks;

public interface ISessionRepository
{
    Task<List<Session>> GetAllSessionsAsync();
    Task AddSessionAsync(Session session);
    Task SaveChangesAsync();
}

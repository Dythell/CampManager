using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SessionRepository : ISessionRepository
{
    private readonly ApplicationDbContext _context;

    public SessionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Session>> GetAllSessionsAsync()
    {
        return await _context.Sessions.ToListAsync();
    }

    public async Task<Session?> GetSessionByIdAsync(int id)
    {
        return await _context.Sessions.FindAsync(id);
    }

    public async Task AddSessionAsync(Session session)
    {
        await _context.Sessions.AddAsync(session);
    }

    public Task UpdateSessionAsync(Session session)
    {
        _context.Sessions.Update(session);
        return Task.CompletedTask;
    }

    public Task DeleteSessionAsync(Session session)
    {
        _context.Sessions.Remove(session);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

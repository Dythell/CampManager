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

    public async Task AddSessionAsync(Session session)
    {
        await _context.Sessions.AddAsync(session);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

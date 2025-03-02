using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class SessionChildRepository : ISessionChildRepository
{
    private readonly ApplicationDbContext _context;

    public SessionChildRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddSessionChildAsync(SessionChild sessionChild)
    {
        await _context.SessionChildren.AddAsync(sessionChild);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

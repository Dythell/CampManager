using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class ChildRepository : IChildRepository
{
    private readonly ApplicationDbContext _context;

    public ChildRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<Child>> GetAllChildrenAsync()
    {
        return await _context.Children.ToListAsync();
    }

    public async Task AddChildAsync(Child child)
    {
        await _context.Children.AddAsync(child);
    }

    public async Task<Child?> GetChildByIdAsync(int id)
    {
        return await _context.Children.FindAsync(id);
    }

    public Task UpdateChildAsync(Child child)
    {
        _context.Children.Update(child);
        return Task.CompletedTask;
    }

    public Task DeleteChildAsync(Child child)
    {
        _context.Children.Remove(child);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

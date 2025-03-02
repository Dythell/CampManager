using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class ChildRepository : IChildRepository
{
    private readonly ApplicationDbContext _context;

    public ChildRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddChildAsync(Child child)
    {
        await _context.Children.AddAsync(child);
    }

    public async Task<Child> GetChildByIdAsync(int childId)
    {
        return await _context.Children.FirstOrDefaultAsync(c => c.Child_Id == childId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

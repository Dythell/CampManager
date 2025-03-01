using Microsoft.EntityFrameworkCore;

public class EventTemplateRepository : IEventTemplateRepository
{
    private readonly ApplicationDbContext _context;
    public EventTemplateRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<EventTemplate>> GetAllTemplatesAsync()
    {
        return await _context.EventTemplates.ToListAsync();
    }

    public async Task AddTemplateAsync(EventTemplate template)
    {
        await _context.EventTemplates.AddAsync(template);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

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

    public async Task<EventTemplate?> GetTemplateByIdAsync(int id)
    {
        return await _context.EventTemplates.FindAsync(id);
    }
    public async Task AddTemplateAsync(EventTemplate template)
    {
        await _context.EventTemplates.AddAsync(template);
    }

    public Task UpdateTemplateAsync(EventTemplate template)
    {
        _context.EventTemplates.Update(template);
        return Task.CompletedTask;
    }

    public Task DeleteTemplateAsync(EventTemplate template)
    {
        _context.EventTemplates.Remove(template);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;

public interface IEventTemplateRepository
{
    Task<List<EventTemplate>> GetAllTemplatesAsync();
    Task<EventTemplate?> GetTemplateByIdAsync(int id);
    Task AddTemplateAsync(EventTemplate template);
    Task UpdateTemplateAsync(EventTemplate template);
    Task DeleteTemplateAsync(EventTemplate template);
    Task SaveChangesAsync();
}

using System.Collections.Generic;
using System.Threading.Tasks;

public interface IEventTemplateRepository
{
    Task<List<EventTemplate>> GetAllTemplatesAsync();
    Task AddTemplateAsync(EventTemplate template);
    Task SaveChangesAsync();
}

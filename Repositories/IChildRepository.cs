using System.Threading.Tasks;
using System.Collections.Generic;

public interface IChildRepository
{
    Task<List<Child>> GetAllChildrenAsync();
    Task AddChildAsync(Child child);
    Task<Child?> GetChildByIdAsync(int id);
    Task UpdateChildAsync(Child child);
    Task DeleteChildAsync(Child child);
    Task SaveChangesAsync();
}

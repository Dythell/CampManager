using System.Threading.Tasks;
using System.Collections.Generic;

public interface IChildRepository
{
    Task AddChildAsync(Child child);
    Task<Child> GetChildByIdAsync(int childId);
    Task SaveChangesAsync();
}

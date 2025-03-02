using System.Collections.Generic;
using System.Threading.Tasks;

public interface IGroupRepository
{
    Task<List<Group>> GetAllGroupsAsync();
    Task AddGroupAsync(Group group);
    Task SaveChangesAsync();
}

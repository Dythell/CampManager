using System.Collections.Generic;
using System.Threading.Tasks;

namespace CampManager.Repositories
{
    public interface IGroupRepository
    {
        Task<List<Group>> GetAllGroupsAsync();
        Task AddGroupAsync(Group group);
        Task SaveChangesAsync();
    }
}

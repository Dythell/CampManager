using System.Collections.Generic;
using System.Threading.Tasks;

namespace CampManager.Repositories
{
    public interface IGroupRepository
    {
        Task<List<Group>> GetAllGroupsAsync();
        Task<Group?> GetGroupByIdAsync(int id);
        Task AddGroupAsync(Group group);
        Task UpdateGroupAsync(Group group);
        Task DeleteGroupAsync(Group group);
        Task SaveChangesAsync();
    }
}

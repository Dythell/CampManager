using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CampManager.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly ApplicationDbContext _context;

        public GroupRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Group>> GetAllGroupsAsync()
        {
            return await _context.Groups
                .Include(g => g.Session)
                .Include(g => g.SessionCounselor)
                    .ThenInclude(sc => sc.Counselor)
                .ToListAsync();
        }

        public async Task<Group?> GetGroupByIdAsync(int id)
            => await _context.Groups
                .Include(g => g.SessionCounselor)
                    .ThenInclude(sc => sc.Counselor)
                .FirstOrDefaultAsync(g => g.Group_Id == id);

        public async Task AddGroupAsync(Group group)
        {
            await _context.Groups.AddAsync(group);
        }

        public Task UpdateGroupAsync(Group group)
        {
            _context.Groups.Update(group);
            return Task.CompletedTask;
        }
        public Task DeleteGroupAsync(Group group)
        {
            _context.Groups.Remove(group);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

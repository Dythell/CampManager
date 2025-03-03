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

        public async Task AddGroupAsync(Group group)
        {
            await _context.Groups.AddAsync(group);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

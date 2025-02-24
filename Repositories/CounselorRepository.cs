using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CampManager.Repositories
{
    public class CounselorRepository : ICounselorRepository
    {
        private readonly ApplicationDbContext _context;

        public CounselorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddCounselorAsync(Counselor counselor)
        {
            await _context.Counselors.AddAsync(counselor);
        }

        public async Task<Counselor?> GetCounselorByIdAsync(int counselorId)
        {
            return await _context.Counselors.FindAsync(counselorId);
        }
        public async Task<IEnumerable<Counselor>> GetAllCounselorsAsync()
        {
            return await _context.Counselors.ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<Counselor?> GetCounselorByUserIdAsync(int userId)
        {
            return await _context.Counselors.FirstOrDefaultAsync(c => c.Counselor_Id == userId);
        }

    }
}

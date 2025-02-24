using System.Threading.Tasks;

namespace CampManager.Repositories
{
    public interface ICounselorRepository
    {
        Task AddCounselorAsync(Counselor counselor);
        Task<Counselor?> GetCounselorByIdAsync(int counselorId);
        Task<Counselor?> GetCounselorByUserIdAsync(int userId);
        Task SaveChangesAsync();
    }
}

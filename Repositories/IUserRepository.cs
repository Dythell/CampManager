using System.Collections.Generic;
using System.Threading.Tasks;

namespace CampManager.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByIdAsync(int userId);
        Task<bool> UserExistsAsync(string username);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(User user);
        Task SaveChangesAsync();
        Task<User> GetUserWithCounselorAsync(int userId);
    }
}

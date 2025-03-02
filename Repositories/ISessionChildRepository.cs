using System.Threading.Tasks;

public interface ISessionChildRepository
{
    Task AddSessionChildAsync(SessionChild sessionChild);
    Task SaveChangesAsync();
}

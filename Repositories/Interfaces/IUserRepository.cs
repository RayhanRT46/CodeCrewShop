using CodeCrewShop.Models.User;

namespace CodeCrewShop.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<Users>> GetAllAsync();
        Task<Users?> GetByIdAsync(int id);
        Task<Users?> GetByEmailAsync(string email);
        Task AddAsync(Users user);
        void Update(Users user);
        void Delete(Users user);
    }
    public interface ITokenService
    {
        string CreateToken(Users user);
        DateTime GetExpiry();
    }
}

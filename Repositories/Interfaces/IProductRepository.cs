using System.Linq.Expressions;
using CodeCrewShop.Models.Product;

namespace CodeCrewShop.Repositories.Interfaces
{
    public interface IProductRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        //Task FindAsync();
    }
}


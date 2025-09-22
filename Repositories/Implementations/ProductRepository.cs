using System.Linq.Expressions;
using CodeCrewShop.Data;
using CodeCrewShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CodeCrewShop.Repositories.Implementations
{
    public class ProductRepository<T> : IProductRepository<T> where T : class
    {
        protected readonly CodeCrewShopContext _context;
        protected readonly DbSet<T> _dbSet;

        public ProductRepository(CodeCrewShopContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()

        {
            //await _context.Database.EnsureCreatedAsync();
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}

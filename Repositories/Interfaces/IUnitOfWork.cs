using CodeCrewShop.Models.Product;

namespace CodeCrewShop.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        //Task<int> SaveAsync();
        IProductRepository<Category> Categorys { get; }
        IProductRepository<Brand> Brands { get; }
        IProductRepository<Product> Products { get; }
        IProductRepository<ProductImage> ProductImages { get; }
        IProductRepository<ProductReview> ProductReviews { get; }
        IProductRepository<Order> Orders { get; }
        IProductRepository<OrderItem> OrderItems { get; }
        IProductRepository<Cart> Carts { get; }
        IProductRepository<CartItem> CartItems { get; }
        IProductRepository<ProductType> ProductTypes { get; }
        Task<int> CompleteAsync();
    }
}
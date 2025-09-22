using CodeCrewShop.Data;
using CodeCrewShop.Models;
using CodeCrewShop.Models.Product;
using CodeCrewShop.Repositories.Interfaces;

namespace CodeCrewShop.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CodeCrewShopContext _context;

        public IUserRepository Users { get; }
        public IProductRepository<Category> Categorys { get; }
        public IProductRepository<Brand> Brands { get; }
        public IProductRepository<Product> Products { get; }
        public IProductRepository<ProductImage> ProductImages { get; }
        public IProductRepository<ProductAttribute> ProductAttributes { get; }
        public IProductRepository<ProductReview> ProductReviews { get; }
        public IProductRepository<Order> Orders { get; }
        public IProductRepository<OrderItem> OrderItems { get; }
        public IProductRepository<Cart> Carts { get; }
        public IProductRepository<CartItem> CartItems { get; }
        public IProductRepository<ProductType> ProductTypes { get; }

        public UnitOfWork(
            CodeCrewShopContext context,
            IUserRepository users,
            IProductRepository<Category> categorys,
            IProductRepository<Brand> brands,
            IProductRepository<Product> products,
            IProductRepository<ProductImage> productImages,
            IProductRepository<ProductAttribute> productAttributes,
            IProductRepository<ProductReview> productReviews,
            IProductRepository<Order> orders,
            IProductRepository<OrderItem> orderItems,
            IProductRepository<Cart> carts,
            IProductRepository<CartItem> cartItems,
            IProductRepository<ProductType> productTypes
        )
        {
            _context = context;

            Users = users;
            Categorys = categorys;
            Brands = brands;
            Products = products;
            ProductImages = productImages;
            ProductAttributes = productAttributes;
            ProductReviews = productReviews;
            Orders = orders;
            OrderItems = orderItems;
            Carts = carts;
            CartItems = cartItems;
            ProductTypes = productTypes;
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

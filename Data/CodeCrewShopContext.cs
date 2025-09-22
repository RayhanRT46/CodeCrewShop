using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CodeCrewShop.Models.User;
using CodeCrewShop.Models.Product;

namespace CodeCrewShop.Data
{
    public class CodeCrewShopContext : DbContext
    {
        public CodeCrewShopContext (DbContextOptions<CodeCrewShopContext> options)
            : base(options)
        {
        }

        //User DB Context
        public DbSet<Users> User { get; set; } = default!;
        public DbSet<Category> Categorys { get; set; } = default!;
        public DbSet<Brand> Brands { get; set; } = default!;
        public DbSet<Product> Products { get; set; } = default!;
        public DbSet<ProductImage> ProductImages { get; set; } = default!;
        public DbSet<ProductAttribute> ProductAttributes { get; set; } = default!;
        public DbSet<ProductReview> ProductReviews { get; set; } = default!;
        public DbSet<Order> Orders { get; set; } = default!;
        public DbSet<OrderItem> OrderItems { get; set; } = default!;
        public DbSet<Cart> Carts { get; set; } = default!;
        public DbSet<CartItem> CartItems { get; set; } = default!;
        public DbSet<ProductType> ProductTypes { get; set; } = default!;


        //DbContext Configuration (Fluent API)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            });

            modelBuilder.Entity<Category>()
                       .HasOne(c => c.ParentCategory)
                       .WithMany(c => c.SubCategories)
                       .HasForeignKey(c => c.ParentCategoryId)
                       .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                        .HasOne(p => p.Category)
                        .WithMany()
                        .HasForeignKey(p => p.CategoryId)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                        .HasOne(p => p.ProductType)
                        .WithMany(b => b.Products)
                        .HasForeignKey(p => p.ProductTypeId)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                        .HasOne(p => p.Seller)
                        .WithMany()
                        .HasForeignKey(p => p.SellerId)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                        .HasOne(p => p.Brand)
                        .WithMany(b => b.Products)
                        .HasForeignKey(p => p.BrandId)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductImage>()
                        .HasOne(i => i.Product)
                        .WithMany(p => p.Images)
                        .HasForeignKey(i => i.ProductId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductAttribute>()
                        .HasOne(i => i.Product)
                        .WithMany(p => p.Attributes)
                        .HasForeignKey(i => i.ProductId)
                        .OnDelete(DeleteBehavior.Cascade);
            // -------------------------
            // Cart <=> CUSTOMER (One Customer -> Many CartItem)
            // -------------------------

            modelBuilder.Entity<Cart>()
                        .HasOne(c => c.User)
                        .WithMany()
                        .HasForeignKey(c => c.UserId)
                        .OnDelete(DeleteBehavior.Cascade);
            // CARTITEM <=> CART
            modelBuilder.Entity<CartItem>()
                        .HasOne(ci => ci.Cart)
                        .WithMany(c => c.CartItems)
                        .HasForeignKey(ci => ci.CartId)
                        .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<CartItem>()
                        .HasOne(ci => ci.Product)
                        .WithMany()
                        .HasForeignKey(ci => ci.ProductId)
                        .OnDelete(DeleteBehavior.Restrict);
            // -------------------------
            // ORDER <=> CUSTOMER (One Customer -> Many Orders)
            // -------------------------
            modelBuilder.Entity<Order>()
                        .HasOne(o => o.User)
                        .WithMany()
                        .HasForeignKey(o => o.UserId)
                        .OnDelete(DeleteBehavior.Restrict);

            // ORDERITEM <=> ORDER
            modelBuilder.Entity<OrderItem>()
                        .HasOne(oi => oi.Order)
                        .WithMany(o => o.Items)
                        .HasForeignKey(oi => oi.OrderId)
                        .OnDelete(DeleteBehavior.Cascade);

            // ORDERITEM <=> PRODUCT
            modelBuilder.Entity<OrderItem>()
                        .HasOne(oi => oi.Product)
                        .WithMany()
                        .HasForeignKey(oi => oi.ProductId)
                        .OnDelete(DeleteBehavior.Restrict);

            //For delet order with Order Items
            modelBuilder.Entity<Order>()
                        .HasMany(o => o.Items)
                        .WithOne(i => i.Order)
                        .HasForeignKey(i => i.OrderId)
                        .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

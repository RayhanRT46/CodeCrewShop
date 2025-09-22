using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CodeCrewShop.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace CodeCrewShop.Models.Product
{
    public class Product : BaseEntity
    {
        //public int Id { get; set; }
        public string? ProductName { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public decimal? SalePrice { get; set; }
        public int StockQuantity { get; set; }
        public int ProductTypeId { get; set; }
        public ProductType? ProductType { get; set; }

        public string? WeightUnit { get; set; } // kg, g, lb
        public string? UnitQuantity { get; set; } // pieces, liter, meter, etc.

        //Category Foreign Keys
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        //Product Brand
        public int BrandId { get; set; }
        public Brand? Brand { get; set; }

        // Seller Info (FK from Users table)
        public int SellerId { get; set; }
        public Users? Seller { get; set; }

        // Navigation Properties
        public IList<ProductImage>? Images { get; set; }
        public IList<ProductAttribute>? Attributes { get; set; }
        public IList<ProductReview>? Reviews { get; set; }
    }

    public class ProductCreateDto
    {
        public int Id { get; set; }
        [Required]
        public string ProductName { get; set; } = string.Empty;

        public string? Slug { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; } = 0;
        public decimal SalePrice { get; set; } = 0;
        public int StockQuantity { get; set; }
        // Category
        public int? ProductTypeId { get; set; }
        public string? ProductTypeName { get; set; }

        //Weight
        public string? WeightUnit { get; set; }
        public string? UnitQuantity { get; set; }

        // Category
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }

        // Brand
        public int? BrandId { get; set; }
        public string? BrandName { get; set; }

        // Seller Info (FK from Users table)
        public int SellerId { get; set; }

        // Product Images
        public List<ProductImageDto>? Images { get; set; }
        // Product Attributes
        public List<ProductAttributeDto>? Attributes { get; set; }


    }
}

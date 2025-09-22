using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CodeCrewShop.Models.Product
{
    // Hierarchical order
    public class Category
    {
        public int Id { get; set; }

        public string? CategoryName { get; set; }

        // Parent Category (nullable, কারণ root category এর parent নাই)
        public int? ParentCategoryId { get; set; }

        // Navigation Property
        public Category? ParentCategory { get; set; }

        // Child Categories
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    }

    public class CategoryCreateDto
    {
        [Required]
        public string? CategoryName { get; set; }
        [AllowNull]
        public int? ParentCategoryId { get; set; }
    }
}

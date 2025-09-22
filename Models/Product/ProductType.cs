using System.ComponentModel.DataAnnotations;

namespace CodeCrewShop.Models.Product
{
    public class ProductType
    {
        public int Id { get; set; }
        [Required]
        public string? ProductTypeName { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}

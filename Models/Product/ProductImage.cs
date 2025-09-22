namespace CodeCrewShop.Models.Product
{
    public class ProductImage
    {
            public int Id { get; set; }
            public string ImageUrl { get; set; } = string.Empty;

            // Foreign key
            public int ProductId { get; set; }
            public Product? Product { get; set; }
    }

    public class ProductImageDto
    {
        public string ImageUrl { get; set; } = string.Empty;
    }

}

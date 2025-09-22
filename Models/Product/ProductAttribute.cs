namespace CodeCrewShop.Models.Product
{
    public class ProductAttribute
    {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;

            // Foreign key
            public int ProductId { get; set; }
            public Product? Product { get; set; }

    }

    public class ProductAttributeDto
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

}

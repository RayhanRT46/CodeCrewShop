namespace CodeCrewShop.Models.Product
{
    public class Brand
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? LogoUrl { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
    public class BrandCreateDto
    {
        public string? Name { get; set; }
        public string? LogoUrl { get; set; }
    }
}

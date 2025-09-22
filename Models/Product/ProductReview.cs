using CodeCrewShop.Models.User;

namespace CodeCrewShop.Models.Product
{
    public class ProductReview
    {
            public int Id { get; set; }
            public string ReviewerName { get; set; } = string.Empty;
            public string Comment { get; set; } = string.Empty;
            public int Rating { get; set; }

            // Foreign key
            public int ProductId { get; set; }
            public Product? Product { get; set; }

    }

    public class ProductReviewDto
    {
        public string ReviewerName { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; } = 0;
    }

}

using CodeCrewShop.Models.User;

namespace CodeCrewShop.Models.Product
{
    public class Cart : BaseEntity
    {
        public int UserId { get; set; }
        public Users? User { get; set; }
        public CartItem? Item { get; set; }
        public IList<CartItem>? CartItems { get; set; }
    }

    public class CartItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int CartId { get; set; }
        public Cart? Cart { get; set; }
    }

    public class cartDto {
        public int UserId { get; set; }
        public IList<CartItem>? CartItems { get; set; }
    }
    public class CartItemDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public int CartId { get; set; }
    }
}

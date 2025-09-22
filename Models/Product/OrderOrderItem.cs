using CodeCrewShop.Models.User;

namespace CodeCrewShop.Models.Product
{
    public enum OrderStatus
    {
        Pending,
        Shipped,
        Delivered,
        Cancelled,
        Return
    }

    public class Order : BaseEntity
    {
        public int UserId { get; set; }
        public Users? User { get; set; }
        public decimal TotalAmount { get; set; } = 0;
        // Enum type status
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public IList<OrderItem>? Items { get; set; }
    }


    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public Product? Product { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderDto
    {
        public int UserId { get; set; }
        public List<OrderItemDto>? Items { get; set; }
    }


    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }


    public class UpdateOrderStatusDto
    {
        public int OrderId { get; set; }
        public OrderStatus Status { get; set; }
    }




}

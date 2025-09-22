using CodeCrewShop.Data;
using CodeCrewShop.Models.Product;
using CodeCrewShop.Models.User;
using CodeCrewShop.Repositories.Interfaces;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeCrewShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly CodeCrewShopContext _context;

        public OrdersController(IUnitOfWork uow, CodeCrewShopContext context)
        {
            _uow = uow;
            _context = context;
        }

        //All Cetegory Get
        [Route("[action]")]
        [HttpGet]
        public IActionResult GetAllOrder()
        {
            // HttpContext uid claim
            var userIdClaim = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("UserId not found in token");

            var userId = int.Parse(userIdClaim);

            // HttpContext role claim
            var userRole = User.FindFirst("role")?.Value; 

            List<Order> orders;

            if (userRole == "Admin")
            {
                // Admin All order can see
                orders = _context.Orders
                          .Include(o => o.Items)
                          .ToList();
            }
            else
            {
                // Normal user only show this user order
                orders = _context.Orders
                          .Where(o => o.UserId == userId)
                          .Include(o => o.Items)
                          .ToList();
            }

            return Ok(orders);
        }




        //Create Order with OrderItem
        [Route("[action]")]
        [HttpPost]
        public IActionResult CreateOrder([FromBody] OrderDto dto)
        {
            // HttpContext to uid claim
            var userIdClaim = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("UserId not found in token");

            var userId = int.Parse(userIdClaim);

            List<OrderItem> items = new List<OrderItem>();
            decimal totalAmount = 0;

            if (dto.Items != null && dto.Items.Any())
            {
                foreach (var i in dto.Items)
                {
                    var product = _context.Products.Find(i.ProductId);
                    if (product == null)
                        return BadRequest($"Product with Id {i.ProductId} not found");

                    decimal priceToUse;
                    if (product.SalePrice.HasValue)
                    {
                        priceToUse = product.SalePrice.Value;
                    }
                    else if (product.Price.HasValue)
                    {
                        priceToUse = product.Price.Value;
                    }
                    else
                    {
                        return BadRequest($"Product with Id {i.ProductId} does not have a valid price.");
                    }

                    var orderItem = new OrderItem
                    {
                        ProductId = product.Id,
                        ProductName = product.ProductName,
                        Quantity = i.Quantity,  // Client to quantity
                        Price = product.Price ?? 0m  // Price = (decimal)product.Price // DB to price
                    };

                    totalAmount += orderItem.Price * orderItem.Quantity;
                    items.Add(orderItem);
                }
            }

            var order = new Order
            {
                UserId = userId,
                TotalAmount = totalAmount,
                Items = items
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            return Ok(order);
        }

       
        // Update Category
        [Route("[action]/{id}")]
        [HttpPut]
        public async Task<IActionResult> OrderStatusUpdate(UpdateOrderStatusDto dto)
        {
            //var existing = await _uow.Orders.GetByIdAsync(id);
            //if (existing == null) return NotFound();
            //existing.Id = Dto.OrderId;
            var order = await _uow.Orders.GetByIdAsync(dto.OrderId);
            if (order == null) return NotFound("Order not found");

            var oldStatus = order.Status;
            order.Status = dto.Status;

            if (oldStatus != OrderStatus.Delivered && dto.Status == OrderStatus.Delivered)
            {
                foreach (var item in order.Items!)
                {
                    if (item.Product == null) continue;

                    if (item.Product.StockQuantity < item.Quantity)
                        return BadRequest($"Not enough stock for product {item.Product.ProductName}");

                    item.Product.StockQuantity -= item.Quantity;
                    _uow.Products.Update(item.Product);
                }
            }
            else if (oldStatus != OrderStatus.Cancelled && dto.Status == OrderStatus.Cancelled)
            {
                foreach (var item in order.Items!)
                {
                    if (item.Product == null) continue;

                    item.Product.StockQuantity += item.Quantity;
                    _uow.Products.Update(item.Product);
                }
            }

            _uow.Orders.Update(order);
            await _uow.CompleteAsync();

            return Ok(order);
        }


        // Delete Order
        [Route("[action]/{id}")]
        [HttpDelete]
        public async Task<IActionResult> OrderDelete(int id)
        {
            // JWT from uid adn role Finding
            var userIdClaim = User.FindFirst("uid")?.Value;
            var roleClaim = User.FindFirst("role")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(roleClaim))
                return Unauthorized("Invalid token");

            int userId = int.Parse(userIdClaim);

            var existing = await _uow.Orders.GetByIdAsync(id);
            if (existing == null) return NotFound("Order not found");

            //Rule 1: Only Customer role delete
            if (roleClaim != "Customer")
                return Forbid("Only customers And Admin can delete orders");

            //Rule 2: Only User his order deleted
            if (existing.UserId != userId)
                return Forbid("You are not the owner of this order");

            //Rule 3: In 30m deleted
            var timeDiff = DateTime.UtcNow - existing.CreatedAt;
            if (timeDiff.TotalMinutes > 30)
                return BadRequest("Order cannot be deleted after 30 minutes of submission");

            // Cascade Order and OrderItems delete
            _uow.Orders.Delete(existing);
            await _uow.CompleteAsync();

            return Ok("Order deleted successfully");
        }


        //Close Order Controller


    }
}

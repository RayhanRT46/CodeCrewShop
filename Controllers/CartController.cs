using CodeCrewShop.Data;
using CodeCrewShop.Models.Product;
using CodeCrewShop.Repositories.Interfaces;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeCrewShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : Controller
    {

    private readonly IUnitOfWork _uow;
    private readonly CodeCrewShopContext _context;

    public CartController(IUnitOfWork uow, CodeCrewShopContext context)
    {
        _uow = uow;
        _context = context;
    }

            //All Card Get
            [Route("[action]")]
            [HttpGet]
            public IActionResult GetAllCart()
            {
                // HttpContext uid claim
            var userIdClaim = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("UserId not found in token");

            var userId = int.Parse(userIdClaim);

            // HttpContext role claim
            var userRole = User.FindFirst("role")?.Value;

            List<Cart> cart;

            if (userRole == "Admin")
            {
                // Admin All order can see
                cart = _context.Carts
                          .Include(o => o.CartItems)
                          .ToList();
            }
            else
            {
                // Normal user only show this user order
                cart = _context.Carts
                          .Where(o => o.UserId == userId)
                          .Include(o => o.CartItems)
                          .ToList();
            }
            return Ok(cart);
            }

            //Create a Cart
            [Route("[action]")]
            [HttpPost]
            public async Task<IActionResult> CreateCart( cartDto dto)
            {
            var userIdClaim = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("UserId not found in token");

            var userId = int.Parse(userIdClaim);

            List<CartItem> items = new List<CartItem>();
            if (dto.CartItems != null && dto.CartItems.Any())
            {
                foreach (var i in dto.CartItems)
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


                await _uow.Categorys.AddAsync();
                await _uow.CompleteAsync();

                return Ok();
            }

            // Update Category
            [Route("[action]/{id}")]
            [HttpPut]
            public async Task<IActionResult> CategoryUpdate(int id, CategoryCreateDto createDto)
            {
                var existing = await _uow.Categorys.GetByIdAsync(id);
                if (existing == null) return NotFound();
                existing.ParentCategoryId = createDto.ParentCategoryId;
                _uow.Categorys.Update(existing);
                await _uow.CompleteAsync();
                return Ok(existing);
            }

            // Delete Category
            [Route("[action]/{id}")]
            [HttpDelete]
            public async Task<IActionResult> CategoryDelete(int id, CategoryCreateDto category)
            {
                var existing = await _uow.Categorys.GetByIdAsync(id);
                if (category == null) return NotFound();
                existing.CategoryName = category.CategoryName;
                existing.ParentCategoryId = category.ParentCategoryId;
                _uow.Categorys.Delete(existing);
                await _uow.CompleteAsync();
                return Ok();
            }
            //Close Category Controller
        }
    }

}
}

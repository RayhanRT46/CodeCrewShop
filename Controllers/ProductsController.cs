using CodeCrewShop.Data;
using CodeCrewShop.Models.Product;
using CodeCrewShop.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeCrewShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Seller")]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly CodeCrewShopContext _context;

        public ProductsController(IUnitOfWork uow, CodeCrewShopContext context)
        {
            _uow = uow;
            _context = context;
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult GetAllProduct()
        {
            Object Products = _context.Products.Include(c => c.Attributes).Include(c => c.Category).Include(c => c.Images).Include(c => c.Reviews).ToList();
            return Ok(Products);
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // HttpContext থেকে uid claim বের করা
            var userIdClaim = User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Plz Login");

            var userId = int.Parse(userIdClaim);

            // ===== Category =====
            int categoryId;
            if (dto.CategoryId.HasValue)
            {
                var exists = await _context.Categorys.AnyAsync(c => c.Id == dto.CategoryId.Value);
                if (!exists) categoryId = 0; // বা BadRequest
                else categoryId = dto.CategoryId.Value;
            }
            else if (!string.IsNullOrWhiteSpace(dto.CategoryName))
            {
                var newCategory = new Category { CategoryName = dto.CategoryName };
                _context.Categorys.Add(newCategory);
                await _context.SaveChangesAsync();
                categoryId = newCategory.Id;
            }
            else return BadRequest("Category is required");

            // ===== Brand =====
            int brandId;
            if (dto.BrandId.HasValue)
            {
                var exists = await _context.Brands.AnyAsync(b => b.Id == dto.BrandId.Value);
                if (!exists) return BadRequest("Invalid BrandId");
                brandId = dto.BrandId.Value;
            }

            else if (!string.IsNullOrWhiteSpace(dto.BrandName))
            {
                var newBrand = new Brand { Name = dto.BrandName };
                _context.Brands.Add(newBrand);
                await _context.SaveChangesAsync();
                brandId = newBrand.Id;
            }
            else brandId = 0; // optional, If Brand null
             
            //Product Type
            int ProductTypes;
            if (dto.ProductTypeId.HasValue)
            {
                var exists = await _context.ProductTypes.AnyAsync(b => b.Id == dto.ProductTypeId.Value);
                if (!exists) return BadRequest("Invalid ProductTypeId");
                ProductTypes = dto.ProductTypeId.Value;
            }
            else if (!string.IsNullOrWhiteSpace(dto.ProductTypeName))
            {
                var newProductType = new ProductType { ProductTypeName = dto.ProductTypeName };
                _context.ProductTypes.Add(newProductType);
                await _context.SaveChangesAsync();
                ProductTypes = newProductType.Id;
            }
            else ProductTypes = 0; // optional, If ProductType null
            // ===== Product =====
            var product = new Product
            {
                ProductName = dto.ProductName,
                Slug = dto.Slug,
                Description = dto.Description,
                Price = dto.Price,
                SalePrice = dto.SalePrice,
                StockQuantity = dto.StockQuantity,
                WeightUnit = dto.WeightUnit,
                UnitQuantity = dto.UnitQuantity,
                CategoryId = categoryId,
                BrandId = brandId,
                ProductTypeId = ProductTypes,
                SellerId = userId
            };

            // ===== Images =====
            if (dto.Images != null)
            {
                if (product.Images == null)
                    product.Images = new List<ProductImage>();

                foreach (var url in dto.Images)
                {
                    product.Images.Add(new ProductImage
                    {
                        ImageUrl = url.ImageUrl
                    });
                }
            }


            // ===== Attributes =====
            if (dto.Attributes != null)
            {
                if (product.Attributes == null)
                    product.Attributes = new List<ProductAttribute>();
                foreach (var attr in dto.Attributes)
                {
                    product.Attributes.Add(new ProductAttribute
                    {
                        Name = attr.Name,
                        Value = attr.Value
                    });
                }
            }

            //// ===== Reviews =====
            //if (dto.Reviews != null)
            //{
            //    foreach (var review in dto.Reviews)
            //    {
            //        product.Reviews.Add(new ProductReview
            //        {
            //            ReviewerName = review.ReviewerName,
            //            Comment = review.Comment,
            //            Rating = review.Rating
            //        });
            //    }
            //}

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }


        // Update Product
        [Route("[action]/{id}")]
        [HttpPut]
        public async Task<IActionResult> ProductUpdate(int id, ProductCreateDto Dto)
        {
            var existing = await _uow.Products.GetByIdAsync(id);
            if (existing == null) return NotFound();
            existing.Id = Dto.Id;
            _uow.Products.Update(existing);
            await _uow.CompleteAsync();
            return Ok(existing);
        }

        // Delete Product
        [Route("[action]/{id}")]
        [HttpDelete]
        public async Task<IActionResult> ProductyDelete(int id)
        {
            var existing = await _uow.Products.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _uow.Products.Delete(existing);
            await _uow.CompleteAsync();
            return Ok();
        }
        //Close Product Controller

    }
}

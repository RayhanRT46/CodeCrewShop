using CodeCrewShop.Data;
using CodeCrewShop.Models.Product;
using CodeCrewShop.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeCrewShop.Controllers
{
    public class BrandsController : ControllerBase
        {
        private readonly IUnitOfWork _uow;
        private readonly CodeCrewShopContext _context;

        public BrandsController(IUnitOfWork uow, CodeCrewShopContext context)
        {
            _uow = uow;
            _context = context;
        }

        //All Brand Get
        [Route("[action]")]
        [HttpGet]
        public IActionResult GetAllBrand()
        {
            Object Brand = _context.Brands.ToList();
            return Ok(Brand);
        }

        //Add a Category
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> CreateBrand(BrandCreateDto dto)
        {
            var v = new Brand
            {
                Name = dto.Name,
                LogoUrl = dto.LogoUrl
            };

            await _uow.Brands.AddAsync(v);
            await _uow.CompleteAsync();

            return Ok(v);
        }

        // Update Brand
        [Route("[action]/{id}")]
        [HttpPut]
        public async Task<IActionResult> ProductBrand(int id, BrandCreateDto createDto)
        {
            var existing = await _uow.Brands.GetByIdAsync(id);
            if (existing == null) return NotFound();
            existing.Name = createDto.Name;
            existing.LogoUrl = createDto.LogoUrl;
            _uow.Brands.Update(existing);
            await _uow.CompleteAsync();
            return Ok(existing);
        }

        // Delete Brand
        [Route("[action]/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeletBrand(int id, BrandCreateDto category)
        {
            var existing = await _uow.Brands.GetByIdAsync(id);
            if (category == null) return NotFound();
            existing.Name = category.Name;
            existing.LogoUrl = category.LogoUrl;
            _uow.Brands.Delete(existing);
            await _uow.CompleteAsync();
            return Ok();
        }
    }
}

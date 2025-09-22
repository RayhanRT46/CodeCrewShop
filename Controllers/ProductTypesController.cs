using CodeCrewShop.Data;
using CodeCrewShop.Models.Product;
using CodeCrewShop.Repositories.Implementations;
using CodeCrewShop.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeCrewShop.Controllers
{
    public class ProductTypesController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly CodeCrewShopContext _context;

        public ProductTypesController(IUnitOfWork uow, CodeCrewShopContext context)
        {
            _uow = uow;
            _context = context;
        }

        //All ProductTypes Get
        [Route("[action]")]
        [HttpGet]
        public IActionResult GetAllProductTypes()
        {
            Object List = _context.ProductTypes.ToList();
            return Ok(List);
        }

        //Add a ProductTypes
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> CreateProductTypes(ProductType dto)
        {
            var v = new ProductType
            {
                ProductTypeName = dto.ProductTypeName
            };

            await _uow.ProductTypes.AddAsync(v);
            await _uow.CompleteAsync();
            return Ok(v);
        }

        //Update ProductTypes
        [Route("[action]/{id}")]
        [HttpPut]
        public async Task<IActionResult> ProductTypes(int id, ProductType createDto)
        {
            var existing = await _uow.ProductTypes.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _uow.ProductTypes.Update(existing);
            await _uow.CompleteAsync();
            return Ok(existing);
        }

        // Delete ProductTypes
        [Route("[action]/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeletProductTypes(int id, ProductType Dto)
        {
            var existing = await _uow.ProductTypes.GetByIdAsync(id);
            if (Dto == null) return NotFound();
            _uow.ProductTypes.Delete(existing);
            await _uow.CompleteAsync();
            return Ok();
        }
    }
}

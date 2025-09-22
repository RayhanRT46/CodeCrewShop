using CodeCrewShop.Data;
using CodeCrewShop.Models;
using CodeCrewShop.Models.Product;
using CodeCrewShop.Repositories;
using CodeCrewShop.Repositories.Implementations;
using CodeCrewShop.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeCrewShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CetegorysController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly CodeCrewShopContext _context;

        public CetegorysController(IUnitOfWork uow , CodeCrewShopContext context)
        {
            _uow = uow;
            _context = context;
        }

        //All Cetegory Get
        [Route("[action]")]
        [HttpGet]
        public IActionResult GetAllCetegory()
        {
            Object categorys = _context.Categorys.Include(c => c.ParentCategory).ToList();
            return Ok(categorys);
        }

        //Add a Category
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody]CategoryCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int? parentId = null; //If exists ar Not Valid then Save with null

            if (dto.ParentCategoryId.HasValue)
            {
                // check if parent exists
                bool exists = await _context.Categorys
                    .AnyAsync(c => c.Id == dto.ParentCategoryId.Value);

                if (exists)
                    parentId = dto.ParentCategoryId.Value;  // If ParentCategoryId valid Then save
            }
            var category = new Category
            {
                CategoryName = dto.CategoryName,
                ParentCategoryId = parentId
            };

            await _uow.Categorys.AddAsync(category);
            await _uow.CompleteAsync();

            return Ok(category);
        }

        // Update Category
        [Route("[action]/{id}")]
        [HttpPut]
        public async Task<IActionResult> CategoryUpdate (int id, CategoryCreateDto createDto)
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

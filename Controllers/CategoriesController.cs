using Microsoft.AspNetCore.Mvc;
using ReposirotyPatternWithUnitOfWork.DTOs;
using ReposirotyPatternWithUnitOfWork.Models;
using ReposirotyPatternWithUnitOfWork.Repositories;
using ReposirotyPatternWithUnitOfWork.Repositories.GenericRepository;

namespace ReposirotyPatternWithUnitOfWork.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        //private readonly ICategoryRepository _categoryRepository;
        private readonly IGenericRepository<Category> _categoryGenericRepository;
        public CategoriesController(IGenericRepository<Category> categoryGenericRepository)
        {
            //_categoryRepository = categoryRepository;
            _categoryGenericRepository = categoryGenericRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            //var categories = await _categoryRepository.GetAllAsync();
            var categories = await _categoryGenericRepository.GetAllAsync();
            var dtos = categories.Select(c => new CategoryDTO
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description
            });
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            //var c = await _categoryRepository.GetByIdAsync(id);
            var c = await _categoryGenericRepository.GetByIdAsync(id);

            if (c == null) return NotFound();
            var dto = new CategoryDTO
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description
            };
            //await _categoryRepository.AddAsync(category);
            //await _categoryRepository.SaveAsync();
            await _categoryGenericRepository.AddAsync(category);
            await _categoryGenericRepository.SaveAsync();
            dto.CategoryId = category.CategoryId; // get generated ID
            return Ok(dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDTO dto)
        {
            if (id != dto.CategoryId)
                return BadRequest("Id mismatch");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //var existing = await _categoryRepository.GetByIdAsync(id);
            var existing = await _categoryGenericRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound();
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            //_categoryRepository.Update(existing);
            //await _categoryRepository.SaveAsync();
            _categoryGenericRepository.Update(existing);
            await _categoryGenericRepository.SaveAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            //var existing = await _categoryRepository.GetByIdAsync(id);
            var existing = await _categoryGenericRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound();
            //_categoryRepository.Delete(existing);
            //await _categoryRepository.SaveAsync();
            _categoryGenericRepository.Delete(existing);
            await _categoryGenericRepository.SaveAsync();
            return NoContent();
        }
    }
}

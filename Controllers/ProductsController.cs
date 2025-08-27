using Microsoft.AspNetCore.Mvc;
using ReposirotyPatternWithUnitOfWork.DTOs;
using ReposirotyPatternWithUnitOfWork.Models;
using ReposirotyPatternWithUnitOfWork.Repositories;
using ReposirotyPatternWithUnitOfWork.Repositories.GenericRepository;

namespace ReposirotyPatternWithUnitOfWork.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<Category> _categoryRepository;
        public ProductsController(IGenericRepository<Product> productRepository, IGenericRepository<Category> categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productRepository.GetAllAsync();
            var dtos = products.Select(p => new ProductDTO
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name
            });
            
            return Ok(dtos);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var p = await _productRepository.GetByIdAsync(id);
            
            if (p == null) return NotFound();
            
            var dto = new ProductDTO
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name
            };
            
            return Ok(dto);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);             
            
            bool categoryExists = await _categoryRepository.ExistAsync(dto.CategoryId);
            if (!categoryExists)
                return BadRequest("Invalid CategoryId");  
            
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                CategoryId = dto.CategoryId
            };
            
            await _productRepository.AddAsync(product);
            await _productRepository.SaveAsync();
            
            dto.ProductId = product.ProductId;
            
            return Ok(dto);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDTO dto)
        {
            if (id != dto.ProductId)
                return BadRequest("Id mismatch");          
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);            
            
            var existing = await _productRepository.GetByIdAsync(id);
            
            if (existing == null)
                return NotFound();
            
            bool categoryExists = await _categoryRepository.ExistAsync(dto.CategoryId);
            if (!categoryExists)
                return BadRequest("Invalid CategoryId");
            
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Price = dto.Price;
            existing.CategoryId = dto.CategoryId;
            
            _productRepository.Update(existing);
            
            await _productRepository.SaveAsync();
            
            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var existing = await _productRepository.GetByIdAsync(id);
            
            if (existing == null) return NotFound();
            
            _productRepository.Delete(existing);
            
            await _productRepository.SaveAsync();
            
            return NoContent();
        }
    }
}

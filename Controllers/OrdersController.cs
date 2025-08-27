using Microsoft.AspNetCore.Mvc;
using ReposirotyPatternWithUnitOfWork.DTOs;
using ReposirotyPatternWithUnitOfWork.Models;
using ReposirotyPatternWithUnitOfWork.Repositories;
using ReposirotyPatternWithUnitOfWork.Repositories.GenericRepository;

namespace ReposirotyPatternWithUnitOfWork.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IGenericRepository<Customer> _customerRepository;
        private readonly IGenericRepository<Product> _productRepository;
        public OrdersController(
            IGenericRepository<Order> orderRepository,
            IGenericRepository<Customer> customerRepository,
            IGenericRepository<Product> productRepository)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderRepository.GetAllAsync();
            var dtos = orders.Select(o => new OrderDTO
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer?.FullName,
                OrderAmount = o.OrderAmount,
                OrderItems = o.OrderItems?.Select(oi => new OrderItemDTO
                {
                    OrderItemId = oi.OrderItemId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList() ?? new List<OrderItemDTO>()
            });
            return Ok(dtos);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var o = await _orderRepository.GetByIdAsync(id);
            if (o == null)
                return NotFound();
            var dto = new OrderDTO
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer?.FullName,
                OrderAmount = o.OrderAmount,
                OrderItems = o.OrderItems?.Select(oi => new OrderItemDTO
                {
                    OrderItemId = oi.OrderItemId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList() ?? new List<OrderItemDTO>()
            };
            return Ok(dto);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            bool customerExists = await _customerRepository.ExistAsync(dto.CustomerId);
            if (!customerExists)
                return BadRequest("Invalid CustomerId");
            foreach (var item in dto.OrderItems)
            {
                bool productExists = await _productRepository.ExistAsync(item.ProductId);
                if (!productExists)
                    return BadRequest($"Invalid ProductId: {item.ProductId}");
            }
            var order = new Order
            {
                CustomerId = dto.CustomerId,
                OrderDate = DateTime.UtcNow,
                OrderAmount = dto.OrderItems.Sum(i => i.Quantity * i.UnitPrice),
                OrderItems = dto.OrderItems.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveAsync();
            dto.OrderId = order.OrderId;
            dto.OrderDate = order.OrderDate;
            return Ok(dto);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderDTO dto)
        {
            if (id != dto.OrderId)
                return BadRequest("Id mismatch");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var existing = await _orderRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound();
            bool customerExists = await _customerRepository.ExistAsync(dto.CustomerId);
            if (!customerExists)
                return BadRequest("Invalid CustomerId");
            foreach (var item in dto.OrderItems)
            {
                bool productExists = await _productRepository.ExistAsync(item.ProductId);
                if (!productExists)
                    return BadRequest($"Invalid ProductId: {item.ProductId}");
            }
            existing.CustomerId = dto.CustomerId;
            existing.OrderDate = dto.OrderDate;
            existing.OrderAmount = dto.OrderItems.Sum(i => i.Quantity * i.UnitPrice);
            // Update OrderItems: Clear existing and add new (simplified)
            existing.OrderItems.Clear();
            existing.OrderItems = dto.OrderItems.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList();
            _orderRepository.Update(existing);
            await _orderRepository.SaveAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var existing = await _orderRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound();
            _orderRepository.Delete(existing);
            await _orderRepository.SaveAsync();
            return NoContent();
        }
    }
}

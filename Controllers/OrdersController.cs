using Microsoft.AspNetCore.Mvc;
using ReposirotyPatternWithUnitOfWork.DTOs;
using ReposirotyPatternWithUnitOfWork.Models;
using ReposirotyPatternWithUnitOfWork.Repositories;
using ReposirotyPatternWithUnitOfWork.Repositories.GenericRepository;
using ReposirotyPatternWithUnitOfWork.UnitOfWork;

namespace ReposirotyPatternWithUnitOfWork.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrdersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _unitOfWork.Orders.GetAllAsync();
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
            var o = await _unitOfWork.Orders.GetByIdAsync(id);
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

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                bool customerExists = await _unitOfWork.Customers.ExistAsync(dto.CustomerId);
                if (!customerExists)
                    return BadRequest("Invalid CustomerId");
                foreach (var item in dto.OrderItems)
                {
                    bool productExists = await _unitOfWork.Products.ExistAsync(item.ProductId);
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
                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.CommitAsync();
                dto.OrderId = order.OrderId;
                dto.OrderDate = order.OrderDate;
                return Ok(dto);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return StatusCode(500, "An error occurred while creating the order.");
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderDTO dto)
        {
            if (id != dto.OrderId)
                return BadRequest("Id mismatch");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _unitOfWork.Orders.GetByIdAsync(id);
            if (existing == null)
                return NotFound();
            bool customerExists = await _unitOfWork.Customers.ExistAsync(dto.CustomerId);
            if (!customerExists)
                return BadRequest("Invalid CustomerId");
            foreach (var item in dto.OrderItems)
            {
                bool productExists = await _unitOfWork.Products.ExistAsync(item.ProductId);
                if (!productExists)
                    return BadRequest($"Invalid ProductId: {item.ProductId}");
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                
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
                _unitOfWork.Orders.Update(existing);
                await _unitOfWork.CommitAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return StatusCode(500, "An error occurred while updating the order.");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var existing = await _unitOfWork.Orders.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                _unitOfWork.Orders.Delete(existing);
                await _unitOfWork.CommitAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return StatusCode(500, "An error occurred while deleting the order.");
            }
        }

        // GET: api/orders/bycustomer/{customerId}
        [HttpGet("bycustomer/{customerId}")]
        public async Task<IActionResult> GetOrdersByCustomer(int customerId)
        {
            var orders = await _unitOfWork.Orders.GetOrdersByCustomerAsync(customerId);
            var dtos = orders.Select(o => new OrderDTO
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer?.FullName,
                OrderAmount = o.OrderAmount,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDTO
                {
                    OrderItemId = oi.OrderItemId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            });
            return Ok(dtos);
        }
        // GET: api/orders/bydaterange?startDate=yyyy-MM-dd&endDate=yyyy-MM-dd
        [HttpGet("bydaterange")]
        public async Task<IActionResult> GetOrdersByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var orders = await _unitOfWork.Orders.GetOrdersByDateRangeAsync(startDate, endDate);
            var dtos = orders.Select(o => new OrderDTO
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer?.FullName,
                OrderAmount = o.OrderAmount,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDTO
                {
                    OrderItemId = oi.OrderItemId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            });
            return Ok(dtos);
        }
    }
}

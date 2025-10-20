using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.Services;
using Payment.Domain.DTOs;

namespace Payment.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _service;

        public OrdersController(OrderService service)
        {
            _service = service;
        }

        // Người dùng tạo order
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var order = await _service.CreateOrderAsync(request);
            return Ok(new { message = "Order created successfully", order });
        }

        [HttpPost("create-or-update")]
        public async Task<IActionResult> CreateOrUpdate([FromBody] CreateOrderRequest request)
        {
            if (request == null || request.UserId <= 0)
                return BadRequest("Invalid request data.");

            var result = await _service.CreateOrUpdateOrderAsync(request);

            if (result.StartsWith("Current plan"))
                return BadRequest(result);

            return Ok(new { message = result });
        }
        // Admin xem tất cả order
        [HttpGet("admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _service.GetAllOrdersAsync();
            return Ok(orders);
        }

        // Người dùng xem đơn hàng của mình
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUser(int userId)
        {
            var orders = await _service.GetOrdersByUserAsync(userId);
            return Ok(orders);
        }

        // Admin cập nhật trạng thái
        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusRequest request)
        {
            var updatedOrder = await _service.UpdateOrderStatusAsync(orderId, request);
            if (updatedOrder == null)
                return NotFound(new { message = "Order not found" });
            return Ok(new { message = "Status updated successfully", order = updatedOrder });
        }
    }
}

using Homework.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

// Đảm bảo Controller có thể truy cập DTO này,
// Nếu nó nằm trong Service.cs, bạn cần di chuyển nó ra ngoài (hoặc đặt ở global namespace)
// public class WebhookResponse { public bool Success { get; set; } public string Message { get; set; } }

namespace Homework.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CopyleaksWebhookController : ControllerBase
    {
        private readonly CopyleaksWebhookService _copyleaksWebhookService;

        public CopyleaksWebhookController(CopyleaksWebhookService copyleaksWebhookService)
        {
            _copyleaksWebhookService = copyleaksWebhookService;
        }

        // --- Controller cho Webhook hoàn thành: /api/CopyleaksWebhook/completed ---
        [HttpPost("completed")]
        public async Task<IActionResult> Completed([FromBody] dynamic data)
        {
            try
            {
                // Lấy kết quả (kiểu WebhookResponse) từ Service
                var result = await _copyleaksWebhookService.HandleCompletedWebhookAsync(data);

                // Dùng thuộc tính .Success (Viết hoa)
                if (!result.Success)
                {
                    // Lỗi xử lý trong business logic, trả về BadRequest hoặc 500
                    // Tùy theo Copyleaks yêu cầu, thường nên trả về 2xx nếu bạn đã nhận webhook.
                    // Ở đây, tôi giữ lại logic ban đầu là BadRequest, nhưng dùng .Success và .Message
                    return BadRequest(new { success = result.Success, message = result.Message });
                }

                // Thành công, trả về 200 OK với body JSON
                return Ok(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                // Lỗi hệ thống không mong muốn
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // --- Controller cho Webhook lỗi: /api/CopyleaksWebhook/error ---
        [HttpPost("error")]
        public async Task<IActionResult> Error([FromBody] dynamic error)
        {
            try
            {
                // Lấy kết quả (kiểu WebhookResponse) từ Service
                var result = await _copyleaksWebhookService.HandleErrorWebhookAsync(error);

                // Dùng thuộc tính .Success (Viết hoa)
                if (!result.Success)
                {
                    // Lỗi trong quá trình lưu trữ lỗi, vẫn nên thông báo lỗi
                    return BadRequest(new { success = result.Success, message = result.Message });
                }

                // Lưu trữ lỗi thành công, trả về 200 OK (theo convention)
                return Ok(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                // Lỗi hệ thống không mong muốn
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
using Front_end.Models;
using Front_end.Services.Interfaces;
using System.Net.Http.Json; // Cần dùng thư viện này
using System.Net;
using System.Text.Json; // Để kiểm tra mã trạng thái

namespace Front_end.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;

        public OrderService(HttpClient http)
        {
            _httpClient = http;
            // Dòng này nên được cấu hình bằng IHttpClientFactory trong Program.cs
            _httpClient.BaseAddress = new Uri("https://localhost:7184/");
        }

        // ===================================
        // Lấy tất cả đơn hàng (Admin)
        // ENDPOINT: GET api/orders/admin
        // ===================================
        public async Task<List<OrderResponse>> GetAllAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<OrderResponse>>("api/orders/admin");
                return result ?? new List<OrderResponse>();
            }
            catch
            {
                return new List<OrderResponse>();
            }
        }

        // Lấy chi tiết 1 order (Giữ nguyên)
        public async Task<OrderResponse?> GetByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<OrderResponse>($"api/orders/{id}");
            }
            catch
            {
                return null;
            }
        }

        // ===================================
        // Tạo hoặc Cập nhật đơn hàng
        // ENDPOINT: POST api/orders/create-or-update
        // Trả về true nếu 200 OK, false nếu 400 Bad Request (lỗi nghiệp vụ) hoặc lỗi khác
        // ===================================
        public async Task<string> CreateOrderAndGetMessageAsync(CreateOrderRequest request)
        {
            try
            {
                // Sử dụng endpoint "api/orders/create-or-update"
                var response = await _httpClient.PostAsJsonAsync("api/orders/create-or-update", request);

                // 1. TRƯỜNG HỢP THÀNH CÔNG (200 OK)
                if (response.IsSuccessStatusCode)
                {
                    // Trả về NULL để biểu thị thành công (vì kiểu trả về là string)
                    return null;
                }

                // 2. TRƯỜNG HỢP THẤT BẠI (4xx, 5xx)
                // Đọc nội dung phản hồi
                var responseContent = await response.Content.ReadAsStringAsync();

                // Cố gắng trích xuất thông báo lỗi từ JSON object { "message": "..." }
                try
                {
                    using (var doc = JsonDocument.Parse(responseContent))
                    {
                        if (doc.RootElement.TryGetProperty("message", out var messageElement) && messageElement.ValueKind == JsonValueKind.String)
                        {
                            // Trả về thông báo lỗi chi tiết từ backend
                            return messageElement.GetString();
                        }
                    }
                }
                catch (JsonException)
                {
                    // Nếu không phải JSON hợp lệ, giả định toàn bộ nội dung là chuỗi lỗi
                    // Điều này áp dụng cho trường hợp 400 Bad Request trả về chuỗi thuần (ít phổ biến hơn)
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        return responseContent;
                    }
                }

                // 3. LỖI KHÔNG CÓ THÔNG BÁO CỤ THỂ HOẶC LỖI MẠNG CHUNG
                // Trả về một thông báo lỗi dựa trên mã trạng thái HTTP
                return $"An error occurred with status code: {(int)response.StatusCode}.";
            }
            catch (HttpRequestException ex)
            {
                // Lỗi kết nối mạng, DNS, hoặc không thể kết nối đến API
                return $"Connection error: Could not reach the payment service. ({ex.Message})";
            }
            catch (Exception ex)
            {
                // Các lỗi ngoại lệ khác
                return $"An unexpected error occurred: {ex.Message}";
            }
        }
        // Cập nhật đơn hàng (Giữ nguyên endpoint cũ cho mục đích khác)
        public async Task<bool> UpdateAsync(int id, UpdateOrderRequest request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/orders/{id}/status", request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}

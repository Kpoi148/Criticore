using Front_end.Models;

namespace Front_end.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderResponse>> GetAllAsync();
        Task<OrderResponse> GetByIdAsync(int id);
        Task<string> CreateOrderAndGetMessageAsync(CreateOrderRequest request);
        Task<bool> UpdateAsync(int id, UpdateOrderRequest request);
    }
}

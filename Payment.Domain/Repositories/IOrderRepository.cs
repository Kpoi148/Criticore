using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payment.Domain.Entities;

namespace Payment.Domain.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> GetPendingOrderAsync(int userId);
        Task<Order?> GetLastPaidOrderAsync(int userId);
        Task AddOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task SaveChangesAsync();

        Task<Order> CreateOrderAsync(Order order);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<IEnumerable<Order>> GetOrdersByUserAsync(int userId);
        Task<Order?> GetByIdAsync(int id);
        Task UpdateAsync(Order order);
    }

}

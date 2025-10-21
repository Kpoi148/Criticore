using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Payment.Domain.Entities;
using Payment.Domain.Repositories;
using Payment.Infrastructure.Models;

namespace Payment.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;

        public OrderRepository(OrderDbContext context)
        {
            _context = context;
        }
        public async Task<Order?> GetPendingOrderAsync(int userId)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o => o.UserId == userId && o.Status == "Pending");
        }

        public async Task<Order?> GetLastPaidOrderAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId && o.Status == "Paid")
                .OrderByDescending(o => o.UpdatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }
        // Cập nhật nêu có đơn hàng chưa được xử li
        public Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        //public async Task<Order> CreateOrderAsync(Order order)
        //{
        //    _context.Orders.Add(order);
        //    await _context.SaveChangesAsync();
        //    return order;
        //}

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders.FindAsync(id);
        }

        //public async Task UpdateAsync(Order order)
        //{
        //    _context.Orders.Update(order);
        //    await _context.SaveChangesAsync();
        //}
    }
}

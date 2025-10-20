using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payment.Domain.DTOs;
using Payment.Domain.Entities;
using Payment.Domain.Repositories;

namespace Payment.Application.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _repo;

        public OrderService(IOrderRepository repo)
        {
            _repo = repo;
        }

        public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
        {
            decimal price = 100000;
            int duration = 1;

            switch (request.PackageName)
            {
                case "Gói 6 tháng":
                    price = 500000;
                    duration = 6;
                    break;
                case "Gói 12 tháng":
                case "Gói 1 năm":
                    price = 900000;
                    duration = 12;
                    break;
            }

            var order = new Order
            {
                UserId = request.UserId,
                PackageName = request.PackageName,
                Price = price,
                DurationInMonths = duration,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            return await _repo.CreateOrderAsync(order);
        }
        public async Task<string> CreateOrUpdateOrderAsync(CreateOrderRequest request)
        {
            // Kiểm tra đơn Pending
            var pending = await _repo.GetPendingOrderAsync(request.UserId);
            if (pending != null)
            {
                pending.PackageName = request.PackageName;
                pending.Price = request.Price;
                pending.DurationInMonths = request.DurationInMonths;
                pending.UpdatedAt = DateTime.Now;

                await _repo.UpdateOrderAsync(pending);
                await _repo.SaveChangesAsync();

                return "Updated existing pending order.";
            }

            // Kiểm tra đơn Paid
            var lastPaid = await _repo.GetLastPaidOrderAsync(request.UserId);
            if (lastPaid.UpdatedAt.HasValue)
            {
                var expiredDate = lastPaid.UpdatedAt.Value.AddMonths(lastPaid.DurationInMonths);
                if (DateTime.Now < expiredDate)
                {
                    return $"Current plan still active until {expiredDate:dd/MM/yyyy}. Cannot create new order.";
                }
            }


            // Tạo mới
            var newOrder = new Order
            {
                UserId = request.UserId,
                PackageName = request.PackageName,
                Price = request.Price,
                DurationInMonths = request.DurationInMonths,
                Status = "Pending",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _repo.AddOrderAsync(newOrder);
            await _repo.SaveChangesAsync();

            return "New order created.";
        }
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _repo.GetAllOrdersAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserAsync(int userId)
        {
            return await _repo.GetOrdersByUserAsync(userId);
        }

        public async Task<Order?> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request)
        {
            var order = await _repo.GetByIdAsync(orderId);
            if (order == null)
                return null;

            order.Status = request.Status;
            order.ConfirmedBy = request.AdminID;
            order.UpdatedAt = DateTime.Now;

            if (request.Status == "Paid")
            {
                order.StartDate = DateTime.Now;
                order.EndDate = order.StartDate.Value.AddMonths(order.DurationInMonths);
            }

            await _repo.UpdateAsync(order);
            return order;
        }
    }
}

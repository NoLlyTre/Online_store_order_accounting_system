using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderSystemEF.Data;
using OrderSystemEF.Models;

namespace OrderSystemEF.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetUserOrdersAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> CreateOrderAsync(int userId, List<(int ProductId, int Quantity)> items, string shippingAddress, string phoneNumber)
        {
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = "Pending",
                ShippingAddress = shippingAddress,
                PhoneNumber = phoneNumber,
                TotalAmount = 0
            };

            decimal totalAmount = 0;
            foreach (var (productId, quantity) in items)
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null || product.StockQuantity < quantity)
                {
                    return null;
                }

                var orderItem = new OrderItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = product.Price
                };

                order.OrderItems.Add(orderItem);
                totalAmount += product.Price * quantity;
                product.StockQuantity -= quantity;
            }

            order.TotalAmount = totalAmount;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return false;
            }

            order.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<bool> DeleteOrderAsync(int orderId, int userId)
        {
            // Проверяем, что заказ принадлежит пользователю
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
            
            if (order == null)
            {
                return false;
            }

            // Возвращаем товары на склад
            foreach (var orderItem in order.OrderItems)
            {
                var product = await _context.Products.FindAsync(orderItem.ProductId);
                if (product != null)
                {
                    product.StockQuantity += orderItem.Quantity;
                }
            }

            // Удаляем заказ (OrderItems удалятся каскадно)
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}


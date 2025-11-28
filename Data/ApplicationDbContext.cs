using System;
using Microsoft.EntityFrameworkCore;
using OrderSystemEF.Models;

namespace OrderSystemEF.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=ordersystem.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Seed initial data - Admin user
            // Password: admin, Hash: jGl25bMBBhy96bQuAoHP5RTu+fsqGwN8F0w86QNoO2U=
            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    Id = 1, 
                    Username = "admin", 
                    Email = "admin@gmail.com", 
                    PasswordHash = "jGl25bMBBhy96bQuAoHP5RTu+fsqGwN8F0w86QNoO2U=", 
                    Role = "Admin", 
                    CreatedAt = DateTime.Now 
                }
            );
            
            // Seed initial products
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Ноутбук", Description = "Игровой ноутбук", Price = 89999, StockQuantity = 10, Category = "Электроника" },
                new Product { Id = 2, Name = "Смартфон", Description = "Флагманский смартфон", Price = 59999, StockQuantity = 20, Category = "Электроника" },
                new Product { Id = 3, Name = "Наушники", Description = "Беспроводные наушники", Price = 4999, StockQuantity = 50, Category = "Аксессуары" },
                new Product { Id = 4, Name = "Планшет", Description = "10-дюймовый планшет с высоким разрешением", Price = 24999, StockQuantity = 15, Category = "Электроника" },
                new Product { Id = 5, Name = "Клавиатура", Description = "Механическая игровая клавиатура", Price = 6999, StockQuantity = 30, Category = "Периферия" },
                new Product { Id = 6, Name = "Мышь", Description = "Беспроводная игровая мышь", Price = 3499, StockQuantity = 40, Category = "Периферия" },
                new Product { Id = 7, Name = "Монитор", Description = "27-дюймовый 4K монитор", Price = 34999, StockQuantity = 12, Category = "Электроника" },
                new Product { Id = 8, Name = "Веб-камера", Description = "Full HD веб-камера с микрофоном", Price = 5999, StockQuantity = 25, Category = "Периферия" },
                new Product { Id = 9, Name = "Колонки", Description = "Стерео колонки с сабвуфером", Price = 8999, StockQuantity = 18, Category = "Аксессуары" },
                new Product { Id = 10, Name = "Внешний жесткий диск", Description = "1TB USB 3.0 внешний жесткий диск", Price = 4999, StockQuantity = 35, Category = "Накопители" }
            );
        }
    }
}


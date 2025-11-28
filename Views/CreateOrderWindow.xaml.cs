using System.Collections.Generic;
using System.Windows;
using OrderSystemEF.Data;
using OrderSystemEF.Models;
using OrderSystemEF.Services;

namespace OrderSystemEF.Views
{
    public partial class CreateOrderWindow : Window
    {
        private readonly User _user;
        private readonly ApplicationDbContext _context;
        private readonly Product _product;
        private readonly OrderService _orderService;

        public CreateOrderWindow(User user, ApplicationDbContext context, Product product)
        {
            InitializeComponent();
            _user = user;
            _context = context;
            _product = product;
            _orderService = new OrderService(_context);

            ProductNameTextBlock.Text = product.Name;
            ProductPriceTextBlock.Text = $"{product.Price:C}";
        }

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(ShippingAddressTextBox.Text))
            {
                MessageBox.Show("Введите адрес доставки", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(PhoneNumberTextBox.Text))
            {
                MessageBox.Show("Введите номер телефона", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var items = new List<(int ProductId, int Quantity)> { (_product.Id, quantity) };
            var order = await _orderService.CreateOrderAsync(
                _user.Id, 
                items, 
                ShippingAddressTextBox.Text, 
                PhoneNumberTextBox.Text
            );

            if (order != null)
            {
                MessageBox.Show($"Заказ создан успешно! Сумма: {order.TotalAmount:C}", 
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Недостаточно товара на складе", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}


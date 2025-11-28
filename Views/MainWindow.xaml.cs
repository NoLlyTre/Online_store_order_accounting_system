using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using OrderSystemEF.Data;
using OrderSystemEF.Models;
using OrderSystemEF.Services;
using OrderSystemEF.Views;

namespace OrderSystemEF.Views
{
    public partial class MainWindow : Window
    {
        private readonly User _currentUser;
        private readonly ApplicationDbContext _context;
        private readonly OrderService _orderService;
        private ObservableCollection<Product>? _products;
        private ObservableCollection<Order>? _orders;

        public MainWindow(User user, ApplicationDbContext context)
        {
            InitializeComponent();
            _currentUser = user;
            _context = context;
            _orderService = new OrderService(_context);
            
            Title = $"Система учета заказов - {user.Username} ({user.Role})";
            LoadProducts();
            LoadOrders();
        }

        private async void LoadProducts()
        {
            var products = await _orderService.GetProductsAsync();
            _products = new ObservableCollection<Product>(products);
            ProductsDataGrid.ItemsSource = _products;
        }

        private async void LoadOrders()
        {
            var orders = await _orderService.GetUserOrdersAsync(_currentUser.Id);
            _orders = new ObservableCollection<Order>(orders);
            OrdersDataGrid.ItemsSource = _orders;
        }

        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ProductsDataGrid.SelectedItem as Product;
            if (selectedProduct == null)
            {
                MessageBox.Show("Выберите товар из списка", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var createOrderWindow = new CreateOrderWindow(_currentUser, _context, selectedProduct);
            if (createOrderWindow.ShowDialog() == true)
            {
                LoadOrders();
                LoadProducts();
            }
        }

        private void MyOrdersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedItem = OrdersTab;
            LoadOrders();
        }

        private async void AllOrdersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser.Role != "Admin")
            {
                MessageBox.Show("Доступ запрещен", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var orders = await _orderService.GetAllOrdersAsync();
            var adminWindow = new AdminOrdersWindow(orders, _context);
            adminWindow.ShowDialog();
        }

        private void ProductsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedItem = ProductsTab;
        }

        private async void DeleteOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrder = OrdersDataGrid.SelectedItem as Order;
            if (selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ для удаления", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить заказ №{selectedOrder.Id}?\nТовары будут возвращены на склад.",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var success = await _orderService.DeleteOrderAsync(selectedOrder.Id, _currentUser.Id);
                
                if (success)
                {
                    MessageBox.Show("Заказ успешно удален", "Успех", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadOrders();
                    LoadProducts();
                }
                else
                {
                    MessageBox.Show("Не удалось удалить заказ", "Ошибка", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LogoutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}


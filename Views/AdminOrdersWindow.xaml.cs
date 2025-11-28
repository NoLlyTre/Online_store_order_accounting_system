using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using OrderSystemEF.Data;
using OrderSystemEF.Models;

namespace OrderSystemEF.Views
{
    public partial class AdminOrdersWindow : Window
    {
        public AdminOrdersWindow(List<Order> orders, ApplicationDbContext context)
        {
            InitializeComponent();
            OrdersDataGrid.ItemsSource = new ObservableCollection<Order>(orders);
        }
    }
}


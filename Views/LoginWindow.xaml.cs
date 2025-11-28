using System.Windows;
using Microsoft.EntityFrameworkCore;
using OrderSystemEF.Data;
using OrderSystemEF.Services;
using OrderSystemEF.Views;

namespace OrderSystemEF.Views
{
    public partial class LoginWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthService _authService;

        public LoginWindow()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _context.Database.EnsureCreated();
            _authService = new AuthService(_context);
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text;
            var password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ShowError("Заполните все поля");
                return;
            }

            var user = await _authService.LoginAsync(username, password);
            if (user != null)
            {
                var mainWindow = new MainWindow(user, _context);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                ShowError("Неверное имя пользователя или пароль");
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow(_context);
            registerWindow.ShowDialog();
        }

        private void ShowError(string message)
        {
            ErrorTextBlock.Text = message;
            ErrorTextBlock.Visibility = Visibility.Visible;
        }
    }
}




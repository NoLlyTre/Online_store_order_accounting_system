using System.Text.RegularExpressions;
using System.Windows;
using OrderSystemEF.Data;
using OrderSystemEF.Services;

namespace OrderSystemEF.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthService _authService;

        public RegisterWindow(ApplicationDbContext context)
        {
            InitializeComponent();
            _context = context;
            _authService = new AuthService(_context);
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text;
            var email = EmailTextBox.Text;
            var password = PasswordBox.Password;
            var confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || 
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                ShowError("Заполните все поля");
                return;
            }

            if (password != confirmPassword)
            {
                ShowError("Пароли не совпадают");
                return;
            }

            if (!IsValidEmail(email))
            {
                ShowError("Некорректный email");
                return;
            }

            var success = await _authService.RegisterAsync(username, email, password);
            if (success)
            {
                MessageBox.Show("Регистрация успешна! Теперь вы можете войти.", "Успех", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                ShowError("Пользователь с таким именем или email уже существует");
            }
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private void ShowError(string message)
        {
            ErrorTextBlock.Text = message;
            ErrorTextBlock.Visibility = Visibility.Visible;
        }
    }
}




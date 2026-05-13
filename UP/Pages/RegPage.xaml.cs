using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UP.Models;


namespace UP.Pages
{
    /// <summary>
    /// Логика взаимодействия для RegPage.xaml
    /// </summary>
    public partial class RegPage : Page
    {
        public RegPage()
        {
            InitializeComponent();
        }
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginTB.Text) ||
                string.IsNullOrWhiteSpace(NameTB.Text) ||
                string.IsNullOrWhiteSpace(PasswordTB.Text) ||
                string.IsNullOrWhiteSpace(EmailTB.Text))
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            if (PasswordTB.Text.Length > 32)
            {
                MessageBox.Show("Пароль должен быть меньше 32 символов");
                return;
            }

            if (!EmailTB.Text.Contains("@"))
            {
                MessageBox.Show("Email должен содержать только цифры и +");
                return;
            }

            var existUser = Core.Context.User.FirstOrDefault(x => x.Login == LoginTB.Text);

            if (existUser != null)
            {
                MessageBox.Show("Такой логин уже существует");
                return;
            }

            var user = new User
            {
                Login = LoginTB.Text,
                Name = NameTB.Text,
                Password = PasswordTB.Text,
                Email = EmailTB.Text,
                RoleID = 1,
                IsFrozen = false
            };

            Core.Context.User.Add(user);
            Core.Context.SaveChanges();

            Core.CurrentUser = user;

            MessageBox.Show("Успешная регистрация!");

            NavigationService.Navigate(new BookCatalog());
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage());
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new BookCatalog());
        }
    }
}

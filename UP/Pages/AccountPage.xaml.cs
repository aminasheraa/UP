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
using UP.Windows;

namespace UP.Pages
{
    /// <summary>
    /// Логика взаимодействия для AccountPage.xaml
    /// </summary>
    public partial class AccountPage : Page
    {
        public AccountPage()
        {
            InitializeComponent();
            LoadUser();
        }

        private void LoadUser()
        {
            var user = Core.CurrentUser;
            if (user == null) return;

            this.DataContext = user;

            ReviewListBox.ItemsSource = Core.Context.Review.Include("Book").Where(r => r.UserID == user.ID).OrderByDescending(r => r.ID).ToList();

            if (user.RoleID == 1)
            {
                AuthorRequestBtn.Visibility = Visibility.Visible;
            }

            if (user.IsFrozen)
            {
                FrozenPanel.Visibility = Visibility.Visible;
                AuthorRequestBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void AuthorRequestBtn_Click(object sender, RoutedEventArgs e)
        {
            RoleApplicationWindow requestWindow = new RoleApplicationWindow();
            requestWindow.ShowDialog();
        }

        private void AppealBtn_Click(object sender, RoutedEventArgs e)
        {
            UnfreezeApplicationWindow UnfreezeApplicationlWindow = new UnfreezeApplicationWindow();
            UnfreezeApplicationlWindow.ShowDialog();
        }        

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти из аккаунта?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Core.CurrentUser = null;

                NavigationService.Navigate(new BookCatalog());

                while (NavigationService.CanGoBack)
                {
                    NavigationService.RemoveBackEntry();
                }
            }
        }
    }
}

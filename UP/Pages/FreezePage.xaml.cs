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
    /// Логика взаимодействия для FreezePage.xaml
    /// </summary>
    public partial class FreezePage : Page
    {
        public FreezePage()
        {
            InitializeComponent();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
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
            Core.CurrentUser = null;
            NavigationService.Navigate(new BookCatalog());
        }

        private void BtnAppeal_Click(object sender, RoutedEventArgs e)
        {
            var win = new Windows.UnfreezeApplicationWindow();
            win.Owner = Window.GetWindow(this);

            if (win.ShowDialog() == true)
            {
                Core.CurrentUser = null;
                NavigationService.Navigate(new BookCatalog());

                while (NavigationService.CanGoBack)
                    NavigationService.RemoveBackEntry();
            }
        }

    }
}

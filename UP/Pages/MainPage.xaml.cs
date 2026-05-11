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

namespace UP.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            LoadData();

        }
        private void LoadData()
        {
            if (Core.CurrentUser != null && Core.CurrentUser.Role != null && Core.CurrentUser.Role.Name == "Автор")
            {
                AccountButton.Visibility = Visibility.Collapsed;
                AuthorButton.Visibility = Visibility.Visible;

            }
            else if (Core.CurrentUser != null && Core.CurrentUser.Role != null && Core.CurrentUser.Role.Name == "Администратор")
            {
                AccountButton.Visibility = Visibility.Collapsed;
                AdminButton.Visibility = Visibility.Visible;
            }
        }
        private void BookCatalogButton_Click(object sender, RoutedEventArgs e)
        {
            MainPageFrame.Navigate(new BookCatalog());
        }
        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser == null)
            {
                MainPageFrame.Navigate(new AuthPage());
            }
            else
            {
                MainPageFrame.Navigate(new AccountPage());
            }
        }
        private void FreezeButton_Click(object sender, RoutedEventArgs e)
        {
        }
        private void ReadingListButton_Click(object sender, RoutedEventArgs e)
        {
        }
        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void AuthorButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

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
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            MainPageFrame.Navigated += (s, e) => LoadData();
        }

        private void LoadData()
        {
            AccountButton.Visibility = Visibility.Visible;
            AuthorButton.Visibility = Visibility.Collapsed;
            AdminButton.Visibility = Visibility.Collapsed;
            FreezeButton.Visibility = Visibility.Collapsed;


            if (Core.CurrentUser != null)
            {
                if (Core.CurrentUser.RoleID == 2)
                {
                    AuthorButton.Visibility = Visibility.Visible;
                    FreezeButton.Visibility = Visibility.Collapsed;

                }
                else if (Core.CurrentUser.RoleID == 3)
                {
                    AccountButton.Visibility = Visibility.Visible;
                    AdminButton.Visibility = Visibility.Visible;
                    FreezeButton.Visibility = Visibility.Collapsed;

                }
            }
            if (Core.CurrentUser?.IsFrozen == true)
            {
                FreezeButton.Visibility = Visibility.Visible;
            }
        }
        private bool IsUserNotNullAndNotFrozen()
        {
            if (Core.CurrentUser == null)
            {
                MessageBox.Show("Необходимо войти в аккаунт!", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            if (Core.CurrentUser.IsFrozen)
            {
                MessageBox.Show("Ваш аккаунт заморожен!", "Заморозка", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;

            }
            return true;
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
            MainPageFrame.Navigate(new FreezePage());
            
        }
        private void ReadingListButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsUserNotNullAndNotFrozen()) return;
            MainPageFrame.Navigate(new ReadingListPage());
        
        }
        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            MainPageFrame.Navigate(new AdminPage());

        }
        private void AuthorButton_Click(object sender, RoutedEventArgs e)
        {
            MainPageFrame.Navigate(new AuthorPage());
        }
    }
}

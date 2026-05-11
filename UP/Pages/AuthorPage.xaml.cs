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
    /// Логика взаимодействия для AuthorPage.xaml
    /// </summary>
    public partial class AuthorPage : Page
    {
        public AuthorPage()
        {
            InitializeComponent();
            UpdateData();
        }

        private void UpdateData()
        {
            if (Core.CurrentUser == null) return;

            var myBooks = Core.Context.Book.Where(b => b.AuthorID == Core.CurrentUser.ID).ToList();
            AuthorBooksListBox.ItemsSource = myBooks;
        }

        private void FrozenElement_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            var book = element.DataContext as Book;

            if (book != null)
            {
                if (book.IsFrozen == false)
                    element.Visibility = Visibility.Collapsed;
                else
                    element.Visibility = Visibility.Visible;
            }
        }

        private void AddBookBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditBookPage(null));
        }

        private void EditBookBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedBook = (sender as Button).DataContext as Book;

            if (selectedBook != null)
            {
                NavigationService.Navigate(new AddEditBookPage(selectedBook));
            }
        }

        private void AppealBookBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedBook = (sender as Button).DataContext as Book;

            if (selectedBook != null)
            {
                UnfreezeApplicationWindow UnfreezeApplicationWindow = new UnfreezeApplicationWindow();
                UnfreezeApplicationWindow.ShowDialog();

                UpdateData();
            }
        }
    }
}

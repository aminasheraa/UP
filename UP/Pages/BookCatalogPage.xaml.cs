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
    /// Логика взаимодействия для BookCatalog.xaml
    /// </summary>
    public partial class BookCatalog : Page
    {
        List<Book> books = new List<Book>();

        public BookCatalog()
        {
            InitializeComponent();
            LoadBooks();
        }
        private void LoadBooks()
        {
             BookListBox.ItemsSource = Core.Context.Book.Include("User").Include("Review").ToList();

        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = SearchTB.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(search))
            {
                LoadBooks();
                return;
            }

            var searchedBook = Core.Context.Book.Where(f => f.Name.ToLower().Contains(search) || f.User.Name.ToLower().Contains(search)).ToList();
            BookListBox.ItemsSource = searchedBook;
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {

            var filtered = Core.Context.Book.ToList();

            if (SortGenreCB.SelectedItem != null)
            {
                string selectedGenre = (SortGenreCB.SelectedItem as ComboBoxItem)?.Content.ToString();

                filtered = filtered.Where(b => b.GenreBook.Any(gb => gb.Genre.Name == selectedGenre)).ToList();
            }

            if (NameAndRatingCB.SelectedIndex == 0)
                filtered = filtered.OrderBy(p => p.Name).ToList();

            if (NameAndRatingCB.SelectedIndex == 1)
                filtered = filtered.OrderByDescending(p => p.Name).ToList();



            if (NameAndRatingCB.SelectedIndex == 2)
                filtered = filtered.OrderBy(p => p.AverageRating).ToList();

            if (NameAndRatingCB.SelectedIndex == 3)
                filtered = filtered.OrderByDescending(p => p.AverageRating).ToList();


            BookListBox.ItemsSource = filtered;
        }

        private void ResetFilter_Click(object sender, RoutedEventArgs e)
        {
            NameAndRatingCB.SelectedIndex = -1;
            SortGenreCB.SelectedIndex = -1;

            LoadBooks();
        }

        private void BookListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }


    }
}

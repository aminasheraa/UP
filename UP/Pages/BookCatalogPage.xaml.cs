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
    /// Логика взаимодействия для BookCatalog.xaml
    /// </summary>
    public partial class BookCatalog : Page
    {
        List<Book> books = new List<Book>();

        public BookCatalog()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateFilters();
        }

        private void UpdateFilters()
        {
            if (BookListBox == null) return; 
            var filtered = Core.Context.Book.Include("User").Include("Review").Where(b => b.IsFrozen == false).ToList();

            string search = SearchTB.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(search))
            {
                filtered = filtered.Where(f => f.Name.ToLower().Contains(search) || f.User.Name.ToLower().Contains(search)).ToList();
            }

            if (SortGenreCB.SelectedIndex > 0) 
            {
                string selectedGenre = (SortGenreCB.SelectedItem as ComboBoxItem)?.Content.ToString();
                filtered = filtered.Where(b => b.GenreBook.Any(gb => gb.Genre.Name == selectedGenre)).ToList();
            }

            switch (NameAndRatingCB.SelectedIndex)
            {
                case 0: 
                    filtered = filtered.OrderBy(p => p.Name).ToList();
                    break;
                case 1: 
                    filtered = filtered.OrderByDescending(p => p.Name).ToList();
                    break;
                case 2: 
                    filtered = filtered.OrderBy(p => p.AverageRating).ToList();
                    break;
                case 3: 
                    filtered = filtered.OrderByDescending(p => p.AverageRating).ToList();
                    break;
            }

            BookListBox.ItemsSource = filtered;
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateFilters();
        }

        private void FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateFilters();
        }

        private void ResetFilter_Click(object sender, RoutedEventArgs e)
        {
            SearchTB.Text = "";
            NameAndRatingCB.SelectedIndex = 0;  
            SortGenreCB.SelectedIndex = 0;
            UpdateFilters();
        }


        private void BookListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (BookListBox.SelectedItem is Book selectedBook)
            {
                NavigationService.Navigate(new BookPage(selectedBook));
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
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!IsUserNotNullAndNotFrozen())
                return;

            var menuItem = sender as MenuItem;
            var selectedBook = menuItem.DataContext as Book;

            if (selectedBook == null) return;

            int sectionId = int.Parse(menuItem.Tag.ToString());

            try
            {
                var existingRecord = Core.Context.ReadingList
                    .FirstOrDefault(rl => rl.UserID == Core.CurrentUser.ID && rl.BookID == selectedBook.ID);

                if (existingRecord == null)
                {
                    ReadingList newListEntry = new ReadingList
                    {
                        UserID = Core.CurrentUser.ID,
                        BookID = selectedBook.ID,
                        SectionID = sectionId
                    };
                    Core.Context.ReadingList.Add(newListEntry);
                    MessageBox.Show($"Книга '{selectedBook.Name}' добавлена в список!");
                }
                else
                {
                    existingRecord.SectionID = sectionId;
                    MessageBox.Show($"Статус книги '{selectedBook.Name}' обновлен.");
                }

                Core.Context.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении: " + ex.Message);
            }
        }

    }
}

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
using System.Data.Entity;
using UP.Models;


namespace UP.Pages
{
    /// <summary>
    /// Логика взаимодействия для ReadingListPage.xaml
    /// </summary>
    public partial class ReadingListPage : Page
    {
        public ReadingListPage()
        {
            InitializeComponent();
            if (SectionTabControl != null)
                SectionTabControl.SelectedIndex = 0;
        }

        private void FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateData();
        }

        private void UpdateData()
        {
            if (SearchTB == null || SectionTabControl == null || ReadingListBox == null)
                return;

            if (Core.CurrentUser == null || Core.Context == null) return;

            var booksQuery = Core.Context.ReadingList
                .Include("Book.User")
                .Include("Book.GenreBook.Genre")
                .Where(rl => rl.UserID == Core.CurrentUser.ID);

            if (SectionTabControl.SelectedItem is TabItem selectedTab && selectedTab.Tag != null)
            {
                if (int.TryParse(selectedTab.Tag.ToString(), out int sectionId))
                {
                    booksQuery = booksQuery.Where(rl => rl.SectionID == sectionId);
                }
            }

            string search = SearchTB.Text?.Trim().ToLower() ?? "";
            if (!string.IsNullOrEmpty(search))
            {
                booksQuery = booksQuery.Where(rl => rl.Book != null &&
                    (rl.Book.Name.ToLower().Contains(search) ||
                     (rl.Book.User != null && rl.Book.User.Name.ToLower().Contains(search))));
            }

            if (SortGenreCB?.SelectedItem != null && SortGenreCB.SelectedIndex > 0)
            {
                string selectedGenre = (SortGenreCB.SelectedItem as ComboBoxItem)?.Content?.ToString();
                if (!string.IsNullOrEmpty(selectedGenre))
                {
                    booksQuery = booksQuery.Where(rl => rl.Book.GenreBook.Any(gb => gb.Genre.Name == selectedGenre));
                }
            }

            var list = booksQuery.ToList();

            if (NameAndRatingCB != null)
            {
                switch (NameAndRatingCB.SelectedIndex)
                {
                    case 0: 
                        list = list.OrderBy(rl => rl.Book?.Name).ToList();
                        break;
                    case 1: 
                        list = list.OrderByDescending(rl => rl.Book?.Name).ToList();
                        break;
                    case 2:
                        list = list.OrderBy(rl => rl.Book?.AverageRating ?? 0).ToList();
                        break;
                    case 3: 
                        list = list.OrderByDescending(rl => rl.Book?.AverageRating ?? 0).ToList();
                        break;
                }
            }

            ReadingListBox.ItemsSource = list;
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateData();
        }

        private void ResetFilter_Click(object sender, RoutedEventArgs e)
        {
            SearchTB.Text = "";
            if (NameAndRatingCB != null) NameAndRatingCB.SelectedIndex = 0;
            if (SortGenreCB != null) SortGenreCB.SelectedIndex = 0;

            UpdateData();
        }

        private void SectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateData();
        }

        private void ReadingListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ReadingListBox.SelectedItem is ReadingList selected && selected.Book != null)
            {
                NavigationService.Navigate(new BookPage(selected.Book));
            }
        }

        private void BtnMove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.ContextMenu != null)
            {
                btn.ContextMenu.PlacementTarget = btn;
                btn.ContextMenu.IsOpen = true;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem menuItem)) return;

            var selectedRecord = menuItem.DataContext as ReadingList;

            if (selectedRecord != null && menuItem.Tag != null)
            {
                try
                {
                    if (int.TryParse(menuItem.Tag.ToString(), out int selectedSectionId))
                    {
                        selectedRecord.SectionID = selectedSectionId;
                        Core.Context.SaveChanges();

                        UpdateData();
                        MessageBox.Show("Книга перемещена!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка сохранения: " + ex.Message);
                }
            }
        }
    }
}

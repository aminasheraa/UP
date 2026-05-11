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
            SectionTabControl.SelectedIndex = 0;
        }

        private void UpdateData()
        {
            if (Core.CurrentUser == null) return;

            var books = Core.Context.ReadingList.Include("Book.User").Where(rl => rl.UserID == Core.CurrentUser.ID && !rl.Book.IsFrozen);

            if (SectionTabControl.SelectedItem is TabItem selectedTab)
            {
                int sectionId = int.Parse(selectedTab.Tag.ToString());
                books = books.Where(rl => rl.SectionID == sectionId);
            }

            string search = SearchTB.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(search))
            {
                books = books.Where(rl => rl.Book.Name.ToLower().Contains(search) || rl.Book.User.Name.ToLower().Contains(search));
            }

            if (SortGenreCB.SelectedItem != null)
            {
                string selectedGenre = (SortGenreCB.SelectedItem as ComboBoxItem)?.Content.ToString();
                books = books.Where(rl => rl.Book.GenreBook.Any(gb => gb.Genre.Name == selectedGenre));
            }

            var list = books.ToList();

            if (NameAndRatingCB.SelectedIndex == 0)
            {
                list = list.OrderBy(rl => rl.Book.Name).ToList();
            }
            else if (NameAndRatingCB.SelectedIndex == 1)
            {
                list = list.OrderByDescending(rl => rl.Book.Name).ToList();
            }
            else if (NameAndRatingCB.SelectedIndex == 2)
            {
                list = list.OrderBy(rl => rl.Book.AverageRating).ToList();
            }
            else if (NameAndRatingCB.SelectedIndex == 3)
            {
                list = list.OrderByDescending(rl => rl.Book.AverageRating).ToList();
            }

            ReadingListBox.ItemsSource = list;
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateData();
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            UpdateData();
        }

        private void ResetFilter_Click(object sender, RoutedEventArgs e)
        {
            SearchTB.Text = "";
            NameAndRatingCB.SelectedIndex = -1;
            SortGenreCB.SelectedIndex = -1;
            UpdateData();
        }

        private void SectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateData();
        }

        private void ReadingListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ReadingListBox.SelectedItem is ReadingList selected)
            {
                NavigationService.Navigate(new BookPage(selected.Book));
            }
        }

        private void BtnMove_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.ContextMenu != null)
            {
                btn.ContextMenu.PlacementTarget = btn;
                btn.ContextMenu.IsOpen = true;
            }
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null) return;

            var contextMenu = menuItem.Parent as ContextMenu;
            var button = contextMenu?.PlacementTarget as Button;

            var selectedRecord = button?.DataContext as ReadingList;

            if (selectedRecord != null)
            {
                try
                {
                    int selectedSectionId = int.Parse(menuItem.Tag.ToString());
                    selectedRecord.SectionID = selectedSectionId;

                    Core.Context.SaveChanges();
                    UpdateData();

                    MessageBox.Show("Книга перемещена!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка сохранения: " + ex.Message);
                }
            }

        }

    }
}

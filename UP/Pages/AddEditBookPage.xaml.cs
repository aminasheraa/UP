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
    /// Логика взаимодействия для AddEditBookPage.xaml
    /// </summary>
    public partial class AddEditBookPage : Page
    {
        private Book _currentBook;

        public AddEditBookPage(Book selectedBook)
        {
            InitializeComponent();

            var allGenres = Core.Context.Genre.ToList();
            GenresListBox.ItemsSource = allGenres;

            if (selectedBook != null)
            {
                _currentBook = selectedBook;
                PageTitle.Text = "Редактирование книги";

                NameTB.Text = _currentBook.Name;
                DescriptionTB.Text = _currentBook.Description;
                BookTextTB.Text = _currentBook.Text;
                ImagePathTB.Text = _currentBook.ImagePath;

                var currentGenreIDs = Core.Context.GenreBook.Where(gb => gb.BookID == _currentBook.ID).Select(gb => gb.GenreID).ToList();

                foreach (Genre genre in allGenres)
                {
                    if (currentGenreIDs.Contains(genre.ID))
                    {
                        GenresListBox.SelectedItems.Add(genre);
                    }
                }
            }
            else
            {
                _currentBook = new Book();
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTB.Text) || string.IsNullOrWhiteSpace(BookTextTB.Text))
            {
                MessageBox.Show("Заполните название и текст книги!");
                return;
            }

            _currentBook.Name = NameTB.Text;
            _currentBook.Description = DescriptionTB.Text;
            _currentBook.Text = BookTextTB.Text;
            _currentBook.ImagePath = ImagePathTB.Text;
            _currentBook.AuthorID = Core.CurrentUser.ID;
            _currentBook.IsFrozen = false;

            if (_currentBook.ID == 0)
                Core.Context.Book.Add(_currentBook);

            try
            {
                Core.Context.SaveChanges();

                var bookID = _currentBook.ID;

                var oldRelations = Core.Context.GenreBook.Where(gb => gb.BookID == bookID).ToList();
                Core.Context.GenreBook.RemoveRange(oldRelations);

                foreach (Genre selectedGenre in GenresListBox.SelectedItems)
                {
                    Core.Context.GenreBook.Add(new GenreBook
                    {
                        BookID = bookID,
                        GenreID = selectedGenre.ID
                    });
                }

                Core.Context.SaveChanges();

                MessageBox.Show("Книга успешно сохранена!");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }



        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}

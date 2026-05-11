using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для BookPage.xaml
    /// </summary>
    public partial class BookPage : Page
    {
        private Book _currentBook;

        public ObservableCollection<Review> BookReviews { get; set; }

        public BookPage(Book selectedBook)
        {
            InitializeComponent();
            _currentBook = selectedBook;
            this.DataContext = _currentBook;
            LoadReviews();


            if (Core.CurrentUser != null && Core.CurrentUser.RoleID == 3)
            {
                AdminPanel.Visibility = Visibility.Visible;
           }
        }
        private void FreezeBtn_Loaded(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (Core.CurrentUser != null && Core.CurrentUser.RoleID == 3)
            {
                btn.Visibility = Visibility.Visible;
            }
            else
            {
                btn.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadReviews()
        {
            if (_currentBook.Review != null)
            {
                var reviews = _currentBook.Review.Where(r => r.IsFrozen == false).OrderByDescending(r => r.CreatedAt);
                BookReviews = new ObservableCollection<Review>(reviews);
                ReviewListBox.ItemsSource = BookReviews;
            }
        }

        private bool IsUserNotNull()
        {
            if (Core.CurrentUser == null)
            {
                MessageBox.Show("Необходимо войти в аккаунт!", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            return true;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new BookCatalog());
        }

        private void BtnSendReview_Click(object sender, RoutedEventArgs e)
        {
            if (!IsUserNotNull()) return; 
            string reviewText = NewReviewText.Text; 
            var selectedRating = NewReviewRating.SelectedItem as ComboBoxItem; 

            if (string.IsNullOrWhiteSpace(reviewText) || selectedRating == null)
            {
                MessageBox.Show("Пожалуйста, напишите текст и выберите оценку.");
                return;
            }

            try
            {
                Review newReview = new Review
                {
                    BookID = _currentBook.ID,
                    UserID = Core.CurrentUser.ID,
                    Text = reviewText,
                    Rating = int.Parse(selectedRating.Content.ToString()),
                    CreatedAt = DateTime.Now,
                    IsFrozen = false
                };

                Core.Context.Review.Add(newReview);
                Core.Context.SaveChanges();

                BookReviews.Insert(0, newReview);

                NewReviewText.Text = "";
                NewReviewRating.SelectedIndex = -1;

                MessageBox.Show("Отзыв успешно добавлен!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }

        private void BtnRead_Click(object sender, RoutedEventArgs e)
        {
            if (!IsUserNotNull()) return;

            NavigationService.Navigate(new ReadPage(_currentBook));
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!IsUserNotNull()) return;

            var menuItem = sender as MenuItem;
            if (menuItem == null) return;

            int selectedSectionId = int.Parse(menuItem.Tag.ToString());
            string sectionName = menuItem.Header.ToString();

            try
            {
                var record = Core.Context.ReadingList.FirstOrDefault(rl => rl.UserID == Core.CurrentUser.ID && rl.BookID == _currentBook.ID);

                if (record == null)
                {
                    ReadingList newList = new ReadingList
                    {
                        UserID = Core.CurrentUser.ID,
                        BookID = _currentBook.ID,
                        SectionID = selectedSectionId
                    };
                    Core.Context.ReadingList.Add(newList);
                }
                else
                {
                    record.SectionID = selectedSectionId;
                }

                Core.Context.SaveChanges();
                MessageBox.Show($"Книга успешно перемещена в список '{sectionName}'!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении в список: " + ex.Message);
            }
        }

        private void BtnComplaint_Click(object sender, RoutedEventArgs e)
        {
            if (!IsUserNotNull()) return;

            var button = sender as Button;
            string target = button.Tag.ToString(); 

            var complaint = new Complaint
            {
                UserID = Core.CurrentUser.ID,
                BookID = _currentBook.ID,
                TargetType = target
            };


            var win = new Windows.ComplaintWindow(complaint);
            win.Owner = Window.GetWindow(this);
            win.ShowDialog();
        }

        private void BtnComplaintReview_Click(object sender, RoutedEventArgs e)
        {
            if (!IsUserNotNull()) return;

            var button = sender as Button;
            var selectedReview = button.DataContext as Review;
            if (selectedReview == null) return;

            var complaint = new Complaint
            {
                UserID = Core.CurrentUser.ID,
                BookID = _currentBook.ID,
                ReviewID = selectedReview.ID,
                TargetType = "Отзыв"
            };

            var win = new Windows.ComplaintWindow(complaint);
            win.Owner = Window.GetWindow(this);
            win.ShowDialog();
        }
        private void BtnFreezeBook_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Заморозить книгу '{_currentBook.Name}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var bookToFreeze = Core.Context.Book.FirstOrDefault(b => b.ID == _currentBook.ID);

                    if (bookToFreeze != null)
                    {
                        bookToFreeze.IsFrozen = true; 

                        Core.Context.SaveChanges();
                        MessageBox.Show("Книга заморожена и больше не будет видна в общем каталоге.");
                        NavigationService.GoBack();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при заморозке книги: " + ex.Message);
                }
            }
        }
        private void BtnFreezeReview_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedReview = button.DataContext as Review;

            if (selectedReview == null) return;

            var result = MessageBox.Show("Заморозить этот отзыв?", "Администрирование", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var review = Core.Context.Review.FirstOrDefault(r => r.ID == selectedReview.ID);

                    if (review != null)
                    {
                        review.IsFrozen = true;
                        Core.Context.SaveChanges();

                        BookReviews.Remove(selectedReview);

                        MessageBox.Show("Отзыв успешно обработан.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

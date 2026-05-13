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
using UP.Models;

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

            BookReviews = new ObservableCollection<Review>();
            ReviewListBox.ItemsSource = BookReviews;

            if (Core.CurrentUser != null && Core.CurrentUser.RoleID == 3)
            {
                AdminPanel.Visibility = Visibility.Visible;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateData();
        }

        private void UpdateData()
        {
            try
            {
                Core.Context.Entry(_currentBook).Reload();
                Core.Context.Entry(_currentBook).Collection(b => b.Review).Load();

                var reviews = _currentBook.Review
                    .Where(r => r.IsFrozen == false)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList();

                BookReviews.Clear();
                foreach (var review in reviews)
                {
                    Core.Context.Entry(review).Reference(r => r.User).Load();
                    BookReviews.Add(review);
                }

                this.DataContext = null;
                this.DataContext = _currentBook;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении данных: " + ex.Message);
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

        private void BtnSendReview_Click(object sender, RoutedEventArgs e)
        {
            if (!IsUserNotNullAndNotFrozen()) return;

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

                UpdateData();

                NewReviewText.Text = "";
                NewReviewRating.SelectedIndex = -1;

                MessageBox.Show("Отзыв успешно добавлен!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack) NavigationService.GoBack();
        }

        private void BtnRead_Click(object sender, RoutedEventArgs e)
        {
            if (!IsUserNotNullAndNotFrozen()) return;
            NavigationService.Navigate(new ReadPage(_currentBook));
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!IsUserNotNullAndNotFrozen()) return;

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
            if (!IsUserNotNullAndNotFrozen()) return;

            var button = sender as Button;
            string target = button?.Tag?.ToString();
            if (string.IsNullOrEmpty(target)) return;

            var complaint = new Complaint { TargetType = target };

            if (target == "Книга")
                complaint.BookID = _currentBook.ID;
            else if (target == "Автор")
                complaint.UserID = _currentBook.AuthorID;

            var win = new Windows.ComplaintWindow(complaint);
            win.Owner = Window.GetWindow(this);
            win.ShowDialog();
        }

        private void BtnComplaintReview_Click(object sender, RoutedEventArgs e)
        {
            if (!IsUserNotNullAndNotFrozen()) return;

            var button = sender as Button;
            var selectedReview = button?.DataContext as Review;
            if (selectedReview == null) return;

            var complaint = new Complaint
            {
                ReviewID = selectedReview.ID,
                TargetType = "Отзыв"
            };

            var win = new Windows.ComplaintWindow(complaint);
            win.Owner = Window.GetWindow(this);
            win.ShowDialog();
        }

        private void BtnFreezeBook_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Заморозить книгу '{_currentBook.Name}'?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _currentBook.IsFrozen = true;
                Core.Context.SaveChanges();
                NavigationService.GoBack();
            }
        }

        private void BtnFreezeReview_Click(object sender, RoutedEventArgs e)
        {
            var selectedReview = (sender as Button)?.DataContext as Review;
            if (selectedReview == null) return;

            if (MessageBox.Show("Заморозить этот отзыв?", "Админ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                selectedReview.IsFrozen = true;
                Core.Context.SaveChanges();
                UpdateData();
            }
        }

        private void FreezeBtn_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
                btn.Visibility = (Core.CurrentUser?.RoleID == 3) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}

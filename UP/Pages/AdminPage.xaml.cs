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
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    /// 
    public partial class AdminPage : Page
    {
        public List<Role> RolesList { get; set; }

        public AdminPage()
        {
            InitializeComponent();
            RolesList = Core.Context.Role.ToList();
            DataContext = this;
        }

        private void SectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e != null && !(e.Source is TabControl))
                return;

            if (SectionTabControl.SelectedItem is TabItem selectedTab)
            {
                string tag = selectedTab.Tag.ToString();

                switch (tag)
                {
                    case "Complaints":
                        ComplaintsListBox.ItemsSource = Core.Context.Complaint.Include("Book").Include("User").Include("Review").ToList();
                        break;
                    case "Unfreeze":
                        UnfreezeListBox.ItemsSource = Core.Context.UnfreezeApplication.Include("User").ToList(); break;
                    case "AuthorApps":
                        AuthorAppsListBox.ItemsSource = Core.Context.RoleApplication.Where(a => a.Status == false).ToList();
                        break;
                    case "Users":
                        UsersListBox.ItemsSource = Core.Context.User.Where(u => u.RoleID != 3).ToList();
                        break;
                    case "FrozenAll":
                        var frozenItems = new List<object>();
                        frozenItems.AddRange(Core.Context.Book.Where(b => b.IsFrozen == true).ToList());
                        frozenItems.AddRange(Core.Context.User.Where(u => u.IsFrozen == true).ToList());
                        frozenItems.AddRange(Core.Context.Review.Include("User").Where(r => r.IsFrozen == true).ToList());
                        FrozenListBox.ItemsSource = frozenItems;
                        break;
                }
            }
        }


        private void QuickUnfreeze_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button).DataContext;

            if (item is Book book)
                book.IsFrozen = false;
            else if (item is User user)
                user.IsFrozen = false;
            else if (item is Review review)
                review.IsFrozen = false;

            try
            {
                Core.Context.SaveChanges();
                MessageBox.Show("Объект успешно разморожен и возвращен в общий доступ.");
                SectionChanged(null, null); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при разморозке: " + ex.Message);
            }
        }



        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            var user = (sender as Button).DataContext as User;

            var dialog = new UP.Windows.ChangePasswordWindow();
            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() == true)
            {
                user.Password = dialog.NewPassword;
                Core.Context.SaveChanges();
                MessageBox.Show("Пароль успешно изменен!");
            }
        }
    
        private void SaveChange_Click(object sender, RoutedEventArgs e) => Core.Context.SaveChanges();
        private void RoleChanged_Click(object sender, SelectionChangedEventArgs e) => Core.Context.SaveChanges();

        private void AcceptBtn_Click(object sender, RoutedEventArgs e)
        {
            var data = (sender as Button).DataContext;

            if (data is Complaint complaint)
            {
                if (complaint.BookID != null)
                {
                    var book = Core.Context.Book.Find(complaint.BookID);
                    if (book != null) book.IsFrozen = true;
                }
                else if (complaint.UserID != null)
                {
                    var user = Core.Context.User.Find(complaint.UserID);
                    if (user != null) user.IsFrozen = true;
                }
                else if (complaint.ReviewID != null)
                {
                    var review = Core.Context.Review.Find(complaint.ReviewID);
                    if (review != null) review.IsFrozen = true;
                }
                if (complaint.ReviewID != null)
                {
                    var review = Core.Context.Review.Find(complaint.ReviewID);
                    if (review != null)
                    {
                        review.IsFrozen = true; 
                    }
                }

                Core.Context.Complaint.Remove(complaint);
            }



            else if (data is UnfreezeApplication ua)
            {
                ua.Status = true; 

                if (ua.BookID != null)
                {
                    var book = Core.Context.Book.FirstOrDefault(b => b.ID == ua.BookID);

                    if (book != null)
                    {
                        book.IsFrozen = false; 
                        Core.Context.Entry(book).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                else if (ua.UserID != null)
                {
                    var user = Core.Context.User.Find(ua.UserID);
                    if (user != null) user.IsFrozen = false;
                }
                try
                {
                    Core.Context.SaveChanges();
                    SectionChanged(null, null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка сохранения: " + ex.Message);
                }
            }

            else if (data is RoleApplication ra)
            {
                ra.Status = true; 
                var user = Core.Context.User.FirstOrDefault(u => u.ID == ra.UserID);
                if (user != null) user.RoleID = 2; 
            }

            Core.Context.SaveChanges();
            SectionChanged(null, null);
        }

        private void RejectBtn_Click(object sender, RoutedEventArgs e)
        {
            var data = (sender as Button).DataContext;

            if (data is RoleApplication ra)
                Core.Context.RoleApplication.Remove(ra);

            else if (data is UnfreezeApplication ua)
                Core.Context.UnfreezeApplication.Remove(ua);

            else if (data is Complaint c)
                Core.Context.Complaint.Remove(c);

            Core.Context.SaveChanges();
            SectionChanged(null, null);
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Core.CurrentUser = null;

            if (NavigationService.CanGoBack)
            {
                while (NavigationService.CanGoBack)
                    NavigationService.RemoveBackEntry();
            }

            NavigationService.Navigate(new AuthPage());
        }
    }
}

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
using System.Windows.Shapes;

namespace UP.Windows
{
    /// <summary>
    /// Логика взаимодействия для RoleApplicationWindow.xaml
    /// </summary>
    public partial class RoleApplicationWindow : Window
    {
        public RoleApplicationWindow()
        {
            InitializeComponent();
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            string description = DescriptionTB.Text.Trim();

            if (description.Length < 20)
            {
                MessageBox.Show("Пожалуйста, напишите более подробное описание (минимум 20 символов).");
                return;
            }

            var newRequest = new RoleApplication
            {
                UserID = Core.CurrentUser.ID,
                Description = description,
                Status = false,
                CreatedAt = DateTime.Now
            };

                Core.Context.RoleApplication.Add(newRequest);
                Core.Context.SaveChanges();

                MessageBox.Show("Ваша заявка на роль Автора успешно отправлена! Администрация рассмотрит её в ближайшее время");
                this.Close();

        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

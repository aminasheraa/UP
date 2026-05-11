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
    /// Логика взаимодействия для UnfreezeApplicationWindow.xaml
    /// </summary>
    public partial class UnfreezeApplicationWindow : Window
    {
        public UnfreezeApplicationWindow()
        {
            InitializeComponent();
        }
        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            string text = DescriptionTB.Text.Trim();
            if (string.IsNullOrEmpty(text))
            {
                MessageBox.Show("Пожалуйста, введите текст обращения");
                return;
            }

 
            var newUnfreezeApplication = new UnfreezeApplication
            {
                UserID = Core.CurrentUser.ID,
                Description = text,
                Status = false,
                CreatedAt = DateTime.Now
            };

            Core.Context.UnfreezeApplication.Add(newUnfreezeApplication);
            Core.Context.SaveChanges();

            MessageBox.Show("Ваша заявка отправлена на рассмотрение!");
            this.Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

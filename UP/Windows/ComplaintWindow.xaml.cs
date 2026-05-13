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


namespace UP.Windows
{
    /// <summary>
    /// Логика взаимодействия для ComplaintWindow.xaml
    /// </summary>
    public partial class ComplaintWindow : Window
    {
        private Complaint _newComplaint;

        public ComplaintWindow(Complaint complaint)
        {
            InitializeComponent();
            _newComplaint = complaint;
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DescriptionTB.Text))
            {
                MessageBox.Show("Пожалуйста, введите текст жалобы.");
                return;
            }

            try
            {
                _newComplaint.Description = DescriptionTB.Text;

                Core.Context.Complaint.Add(_newComplaint);
                Core.Context.SaveChanges();

                MessageBox.Show("Жалоба успешно отправлена!");
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.InnerException?.Message ?? ex.Message;
                MessageBox.Show("Ошибка сохранения: " + message);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}

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
using MassageParlor.DB;

namespace MassageParlor.Windowww
{
    /// <summary>
    /// Логика взаимодействия для AddAppealWindow.xaml
    /// </summary>
    public partial class AddAppealWindow : Window
    {
        Worker loggedWorker;

        public static Appeals appeal = new Appeals();
        public static List<Status> statuses { get; set; }
        public static List<Taskk> tasks { get; set; }

        public AddAppealWindow()
        {
            InitializeComponent();
            loggedWorker = DBConnection.loginedWorker;
            tasks = DBConnection.massageSalon.Taskk.ToList();
            statuses = DBConnection.massageSalon.Status.ToList();
            DateTime currentDateTime = DateTime.Now;
            DateTimeDP.SelectedDate = currentDateTime;
            DateTimeDP.IsEnabled = false;
            StatusTB.Text = "Не выполнено";
            this.DataContext = this;
        }

        private void AddBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TaskCB.SelectedItem == null)
                {
                    MessageBox.Show("Заполните все поля.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                else
                {
                    DateTime currentDateTime = DateTime.Now;
                    appeal.DateTime = currentDateTime;
                    appeal.ID_Worker = loggedWorker.ID;
                    appeal.ID_Status = 1;

                    var a = TaskCB.SelectedItem as Taskk;
                    appeal.ID_Task = a.ID;

                    DBConnection.massageSalon.Appeals.Add(appeal);
                    DBConnection.massageSalon.SaveChanges();
                    Close();
                }
            }
            catch
            {
                MessageBox.Show("Непредвиденная ошибка.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void CancelBTN_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

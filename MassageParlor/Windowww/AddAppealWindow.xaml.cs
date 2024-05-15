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

        public AddAppealWindow()
        {
            InitializeComponent();
            loggedWorker = DBConnection.loginedWorker;
        }

        private void AddBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder error = new StringBuilder();
                if (DateTimeDP.SelectedDate == null || TaskCB.SelectedItem == null)
                {
                    error.AppendLine("Заполните все поля!");
                }
                if (error.Length > 0)
                {
                    MessageBox.Show(error.ToString());
                }
                else
                {
                    appeal.DateTime = ;
                    appeal.ID_Worker = loggedWorker.ID;
                    appeal.Status.Name = "Не выполнено";
                    var a = TaskCB.SelectedItem as Taskk;
                    appeal.ID_Task = a.ID;

                    DBConnection.massageSalon.Appeals.Add(appeal);
                    DBConnection.massageSalon.SaveChanges();
                    Close();
                }
            }
            catch
            {
                MessageBox.Show("Заполните все поля!");
            }
        }
    }
}

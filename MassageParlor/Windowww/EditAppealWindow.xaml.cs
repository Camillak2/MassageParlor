using MassageParlor.DB;
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

namespace MassageParlor.Windowww
{
    /// <summary>
    /// Логика взаимодействия для EditAppealWindow.xaml
    /// </summary>
    public partial class EditAppealWindow : Window
    {
        Appeals contextAppeal;
        public static List<Taskk> tasks { get; set; }
        public static List<Worker> workers { get; set; }

        public EditAppealWindow(Appeals appeal)
        {
            InitializeComponent();
            contextAppeal = appeal;
            InitializeDataInPage();
            this.DataContext = this;
        }

        private void InitializeDataInPage()
        {
            tasks = DBConnection.massageSalon.Taskk.ToList();
            workers = DBConnection.massageSalon.Worker.ToList();
            this.DataContext = this;
            TaskCB.SelectedIndex = (int)contextAppeal.ID_Task;
            DateTimeDP.SelectedDate = contextAppeal.DateTime;
            StatusTB.Text = contextAppeal.Status.Name;
        }

        private void EditBTN_Click(object sender, RoutedEventArgs e)
        {
            TaskCB.IsEditable = true;
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder error = new StringBuilder();
                Appeals appeal = contextAppeal;
                if (TaskCB.SelectedItem == null)
                {
                    error.AppendLine("Заполните все поля!");
                }
                if (error.Length > 0)
                {
                    MessageBox.Show(error.ToString());
                }
                else
                {
                    appeal.ID_Task = (TaskCB.SelectedItem as Taskk).ID;
                    DBConnection.massageSalon.SaveChanges();


                }
                SaveBTN.Visibility = Visibility.Collapsed;
                EditBTN.Visibility = Visibility.Visible;
                Close();
            }
            catch
            {
                MessageBox.Show("Произошла ошибка!");
            }
        }
    }
}

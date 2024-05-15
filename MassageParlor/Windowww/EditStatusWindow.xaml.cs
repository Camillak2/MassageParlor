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
    /// Логика взаимодействия для EditStatusWindow.xaml
    /// </summary>
    public partial class EditStatusWindow : Window
    {
        Appeals contextAppeal;
        public static List<Taskk> tasks { get; set; }
        public static List<Worker> workers { get; set; }
        public static List<Appeals> appeals { get; set; }

        public EditStatusWindow(Appeals appeal)
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
            TaskTB.Text = contextAppeal.Taskk.Name;
            DateTimeDP.SelectedDate = contextAppeal.DateTime;
            WorkerTB.Text = contextAppeal.Worker.Surname + " " + contextAppeal.Worker.Name + " " + contextAppeal.Worker.Patronymic;
            StatusCB.SelectedIndex = (int)contextAppeal.ID_Status;
        }

        private void EditBTN_Click(object sender, RoutedEventArgs e)
        {
            StatusCB.IsEditable = true;
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder error = new StringBuilder();
                Appeals appeal = contextAppeal;
                if (StatusCB.SelectedItem == null)
                {
                    error.AppendLine("Заполните все поля!");
                }
                if (error.Length > 0)
                {
                    MessageBox.Show(error.ToString());
                }
                else
                {
                    appeal.ID_Status = (StatusCB.SelectedItem as Status).ID;
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

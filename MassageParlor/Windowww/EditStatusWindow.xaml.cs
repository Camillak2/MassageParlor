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

        public EditStatusWindow(Appeals appeal)
        {
            InitializeComponent();
            contextAppeal = appeal;
            InitializeDataInPage();
            this.DataContext = this;
        }

        private void InitializeDataInPage()
        {
            TaskTB.Text = contextAppeal.Taskk.Name;
            WorkerTB.Text = contextAppeal.Worker.Surname + " " + contextAppeal.Worker.Name + " " + contextAppeal.Worker.Patronymic;
            tasks = DBConnection.massageSalon.Taskk.ToList();
            workers = DBConnection.massageSalon.Worker.ToList();
            this.DataContext = this;
            SurnameTB.Text = contextClient.Surname;
            NameTB.Text = contextClient.Name;
            PatronymicTB.Text = contextClient.Patronymic;
            DateOfBirthDP.SelectedDate = contextClient.DateOfBirth;
            PhoneTB.Text = contextClient.Phone;
            WorkerCB.SelectedIndex = (int)contextAppeal.ID_Worker - 1;
            StatusCB.SelectedIndex = (int)contextAppeal.ID_Status;
        }

        private void EditBTN_Click(object sender, RoutedEventArgs e)
        {
            StatusCB.IsEditable = true;
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

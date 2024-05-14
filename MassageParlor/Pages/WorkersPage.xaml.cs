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
using MassageParlor.DB;
using MassageParlor.Windowww;

namespace MassageParlor.Pages
{
    /// <summary>
    /// Логика взаимодействия для WorkersPage.xaml
    /// </summary>
    public partial class WorkersPage : Page
    {
        Worker loggedWorker;
        public static List<Worker> workers {  get; set; }
        public static List<Position> positions { get; set; }
        public static List<Gender> genders { get; set; }
        public WorkersPage()
        {
            InitializeComponent();
            loggedWorker = DBConnection.loginedWorker;
            Refresh();
            CheckConditionAndToggleButtonVisibility();
        }

        private void CheckConditionAndToggleButtonVisibility()
        {
            if (loggedWorker.ID_Position == 1)
            {
                WorkersBTN.Visibility = Visibility.Visible;
                MassageBTN.Visibility = Visibility.Collapsed;
            }
            else
            {
                WorkersBTN.Visibility = Visibility.Collapsed;
                MassageBTN.Visibility = Visibility.Visible;
            }
        }

        public void Refresh()
        {
            WorkersLV.ItemsSource = DBConnection.massageSalon.Worker.ToList();
        }

        private void DeleteHL_Click(object sender, RoutedEventArgs e)
        {
            var worker = (sender as Hyperlink).DataContext as Worker;
            try
            {
                DBConnection.massageSalon.Worker.Remove(worker);
                DBConnection.massageSalon.SaveChanges();
            }
            catch
            {
                MessageBox.Show("Сотрудник не может быть удален!");
            }

            Refresh();
        }

        private void ProfileBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MyPersonalAccountPage());
        }

        private void ClientsBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ClientsPage());
        }

        private void RecordsBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RecordsPage());
        }

        private void ServicesBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ServicesPage());
        }

        private void WorkersBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new WorkersPage());
        }

        private void MassageBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MassagePage());
        }

        private void LogOutBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthorizationPage());
        }

        private void AddBTN_Click(object sender, RoutedEventArgs e)
        {
            AddWorkerWindow addWorkerWindow = new AddWorkerWindow();
            addWorkerWindow.ShowDialog();
            Refresh();
        }

        private void EditBTN_Click(object sender, RoutedEventArgs e)
        {
            if (WorkersLV.SelectedItem is Worker worker)
            {
                DBConnection.selectedForEditWorker = WorkersLV.SelectedItem as Worker;
                EditWorkerWindow editWorkerWindow = new EditWorkerWindow(worker);
                editWorkerWindow.ShowDialog();
            }
            else if (WorkersLV.SelectedItem is null)
            {
                MessageBox.Show("Выберите сотрудника!");
            }
            Refresh();
        }

    }
}

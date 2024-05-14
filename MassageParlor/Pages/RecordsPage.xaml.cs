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

namespace MassageParlor.Pages
{
    /// <summary>
    /// Логика взаимодействия для RecordsPage.xaml
    /// </summary>
    public partial class RecordsPage : Page
    {
        Worker loggedWorker;
        public static List<Record> records {  get; set; }
        public RecordsPage()
        {
            InitializeComponent();
            loggedWorker = DBConnection.loginedWorker;
            CheckConditionAndToggleButtonVisibility();
            Refresh();
        }

        public void Refresh()
        {
            RecordsLV.ItemsSource = DBConnection.massageSalon.Record.ToList();
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

        private void DeleteHL_Click(object sender, RoutedEventArgs e)
        {
            var client = (sender as Hyperlink).DataContext as Client;
            try
            {
                DBConnection.massageSalon.Client.Remove(client);
                DBConnection.massageSalon.SaveChanges();
            }
            catch
            {
                MessageBox.Show("Этот клиент не может быть удален!");
            }

            Refresh();
        }

        private void ProfileBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MyPersonalAccountPage());
        }

        private void WorkersBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new WorkersPage());
        }

        private void ClientsBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ClientsPage());
        }

        private void RecordsBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RecordsPage());
        }

        private void MassageBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MassagePage());
        }

        private void ServicesBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ServicesPage());
        }

        private void LogOutBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthorizationPage());
        }

        private void AddBTN_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditBTN_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

using MassageParlor.DB;
using MassageParlor.Windowww;
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

namespace MassageParlor.Pages
{
    /// <summary>
    /// Логика взаимодействия для ClientsPage.xaml
    /// </summary>
    public partial class ClientsPage : Page
    {
        Worker loggedWorker;
        public static List<Client> clients { get; set; }
        public static List<Worker> workers { get; set; }
        public static List<Gender> genders { get; set; }
        public ClientsPage()
        {
            InitializeComponent();
            loggedWorker = DBConnection.loginedWorker;
            Refresh();
            CheckConditionAndToggleButtonVisibility();
        }

        private void CheckConditionAndToggleButtonVisibility()
        {
            if (loggedWorker.Position.Name == "Администратор")
            {
                WorkersBTN.Visibility = Visibility.Visible;
                MassageBTN.Visibility = Visibility.Collapsed;
            }
            else if (loggedWorker.Position.Name == "Массажист")
            {
                WorkersBTN.Visibility = Visibility.Collapsed;
                EditBTN.Visibility = Visibility.Collapsed;
                AddBTN.Visibility = Visibility.Collapsed;
                MassageBTN.Visibility = Visibility.Visible;
            }
        }

        public void Refresh()
        {
            ClientsLV.ItemsSource = DBConnection.massageSalon.Client.ToList();
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
            AddClientWindow addClientWindow = new AddClientWindow();
            addClientWindow.ShowDialog();
            Refresh();
        }

        private void EditBTN_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsLV.SelectedItem is Client client)
            {
                DBConnection.selectedForEditClient = ClientsLV.SelectedItem as Client;
                EditClientWindow editClientWindow = new EditClientWindow(client);
                editClientWindow.ShowDialog();
            }
            else if (ClientsLV.SelectedItem is null)
            {
                MessageBox.Show("Выберите клиента!");
            }
            Refresh();
        }

        private void DeleteHL_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
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
            else if (result == MessageBoxResult.No) { }
        }

        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainMenuPage());
        }
    }
}

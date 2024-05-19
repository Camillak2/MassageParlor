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
        public static List<Gender> genders { get; set; }
        public static List<Worker> workers { get; set; }
        public static List<Record> records { get; set; }

        public ClientsPage()
        {
            InitializeComponent();
            loggedWorker = DBConnection.loginedWorker;
            workers = DBConnection.massageSalon.Worker.ToList();
            genders = DBConnection.massageSalon.Gender.ToList();
            clients = DBConnection.massageSalon.Client.ToList();
            records = DBConnection.massageSalon.Record.ToList();
            Refresh();
            CheckConditionAndToggleButtonVisibility();
        }

        private void CheckConditionAndToggleButtonVisibility()
        {
            if (loggedWorker.Position.Name == "Администратор")
            {
                //Видно
                ProfileBTN.Visibility = Visibility.Visible;
                RecordsBTN.Visibility = Visibility.Visible;
                ServicesBTN.Visibility = Visibility.Visible;
                WorkersBTN.Visibility = Visibility.Visible;
                ClientsBTN.Visibility = Visibility.Visible;
                LogOutBTN.Visibility = Visibility.Visible;

                //Не видно
                MassageBTN.Visibility = Visibility.Collapsed;
            }
            else if (loggedWorker.Position.Name == "Массажист")
            {
                //Видно
                ProfileBTN.Visibility = Visibility.Visible;
                RecordsBTN.Visibility = Visibility.Visible;
                ServicesBTN.Visibility = Visibility.Visible;
                MassageBTN.Visibility = Visibility.Visible;
                LogOutBTN.Visibility = Visibility.Visible;

                //Не видно
                WorkersBTN.Visibility = Visibility.Collapsed;
                ClientsBTN.Visibility = Visibility.Collapsed;
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

        private void ClientsLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClientsLV.SelectedItem is Client client)
            {
                DBConnection.selectedForEditClient = ClientsLV.SelectedItem as Client;
                EditClientWindow editClientWindow = new EditClientWindow(client);
                editClientWindow.ShowDialog();
            }
            else if (ClientsLV.SelectedItem is null)
            {
                MessageBox.Show("Выберите клиента.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            Refresh();
        }

        private bool IsClientExists(Client client)
        {
            return DBConnection.massageSalon.Record.Any(c => c.ID_Client == client.ID);
        }

        private void DeleteHL_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            var client = (sender as Hyperlink).DataContext as Client;
            if (result == MessageBoxResult.Yes)
            {
                if (IsClientExists(client))
                {
                    MessageBox.Show("Этот клиент не может быть удален. Он участвует в записи.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    using (var db = new MassageSalonEntities())
                    {
                        // Находим клиента по его ID
                        var clientToDelete = DBConnection.massageSalon.Client.Find(client.ID);

                        if (clientToDelete != null)
                        {
                            // Удаляем клиента из контекста
                            DBConnection.massageSalon.Client.Remove(client);
                            DBConnection.massageSalon.SaveChanges();


                            Refresh();
                            MessageBox.Show("Клиент удален.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        else
                        {
                            MessageBox.Show("Клиент не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                }
            }
            else if (result == MessageBoxResult.No) { }
        }

        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainMenuPage());
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchTB.Text.Length > 0)

                ClientsLV.ItemsSource = DBConnection.massageSalon.Client.Where(i => i.Surname.ToLower().StartsWith(SearchTB.Text.Trim().ToLower())
                || i.Name.ToLower().StartsWith(SearchTB.Text.Trim().ToLower()) || i.Patronymic.ToLower().StartsWith(SearchTB.Text.Trim().ToLower())).ToList();

            else
                ClientsLV.ItemsSource = DBConnection.massageSalon.Client.ToList();
        }
    }
}

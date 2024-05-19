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
    /// Логика взаимодействия для AllServicesPage.xaml
    /// </summary>
    public partial class AllServicesPage : Page
    {
        Worker loggedWorker;

        public static List<TypeOfService> types { get; set; }
        public static List<Service> services { get; set; }
        public static Service service { get; set; }
        TypeOfService contextType;

        public AllServicesPage(TypeOfService type)
        {
            InitializeComponent();
            contextType = type;
            loggedWorker = DBConnection.loginedWorker;
            CheckConditionAndToggleButtonVisibility();
            Refresh();
            types = DBConnection.massageSalon.TypeOfService.Where(i => i.Name == contextType.Name).ToList();
            ServicesLV.ItemsSource = DBConnection.massageSalon.Service.Where(i => i.ID_TypeOfService == contextType.ID).ToList();
            NameTB.Text = Convert.ToString(contextType.Name);
            services = DBConnection.massageSalon.Service.ToList();
            this.DataContext = this;
        }
        public void Refresh()
        {
            ServicesLV.ItemsSource = DBConnection.massageSalon.Service.Where(i => i.ID_TypeOfService == contextType.ID).ToList();
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

        private void ServicesBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ServicesPage());
        }

        private void MassageBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MassagePage());
        }

        private void LogOutBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthorizationPage());
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
                LogOutBTN.Visibility = Visibility.Visible;

                //Не видно
                WorkersBTN.Visibility = Visibility.Collapsed;
                ClientsBTN.Visibility = Visibility.Collapsed;
                MassageBTN.Visibility = Visibility.Collapsed;
            }
        }

        private void AddBTN_Click(object sender, RoutedEventArgs e)
        {
            AddServiceWindow addServiceWindow = new AddServiceWindow(contextType);
            addServiceWindow.ShowDialog();
            Refresh();
        }

        private void ServicesLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ServicesLV.SelectedItem is Service service)
            {
                DBConnection.selectedForEditService = ServicesLV.SelectedItem as Service;
                EditServiceWindow editServiceWindow = new EditServiceWindow(service);
                editServiceWindow.ShowDialog();
            }
            else if (ServicesLV.SelectedItem is null)
            {
                MessageBox.Show("Выберите услугу!");
            }
            Refresh();
        }

        private void DeleteHL_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var service = (sender as Hyperlink).DataContext as Service;
                try
                {
                    DBConnection.massageSalon.Service.Remove(service);
                    DBConnection.massageSalon.SaveChanges();
                }
                catch
                {
                    MessageBox.Show("Эта услуга не может быть удалена!");
                }

                Refresh();
            }
            else if (result == MessageBoxResult.No) { }
        }

        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ServicesPage());
        }
    }
}

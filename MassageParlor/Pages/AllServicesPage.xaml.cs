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
        public void Refresh1()
        {
            ServicesForMassagistLV.ItemsSource = DBConnection.massageSalon.Service.Where(i => i.ID_TypeOfService == contextType.ID).ToList();
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
                ServicesLV.Visibility = Visibility.Visible;

                AddBTN.Visibility = Visibility.Visible;
                LogOutBTN.Visibility = Visibility.Visible;

                //Не видно
                MassageBTN.Visibility = Visibility.Collapsed;
                ServicesForMassagistLV.Visibility = Visibility.Collapsed;


                Refresh();
            }
            else if (loggedWorker.Position.Name == "Массажист")
            {
                //Видно
                ProfileBTN.Visibility = Visibility.Visible;
                RecordsBTN.Visibility = Visibility.Visible;
                ServicesBTN.Visibility = Visibility.Visible;
                MassageBTN.Visibility = Visibility.Visible;
                LogOutBTN.Visibility = Visibility.Visible;
                ServicesForMassagistLV.Visibility = Visibility.Visible;

                //Не видно
                WorkersBTN.Visibility = Visibility.Collapsed;
                ClientsBTN.Visibility = Visibility.Collapsed;
                ServicesLV.Visibility = Visibility.Collapsed;
                AddBTN.Visibility = Visibility.Collapsed;


                Refresh1();
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

        private bool IsServiceExists(Service service)
        {
            return DBConnection.massageSalon.Record.Any(c => c.ID_Service == service.ID);
        }

        private void DeleteHL_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            var service = (sender as Hyperlink).DataContext as Service;
            if (result == MessageBoxResult.Yes)
            {
                if (IsServiceExists(service))
                {
                    MessageBox.Show("Эта услуга не может быть удалена. Она участвует в записи.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    using (var db = new MassageSalonEntities())
                    {
                        // Находим клиента по его ID
                        var serviceToDelete = DBConnection.massageSalon.Service.Find(service.ID);

                        if (serviceToDelete != null)
                        {
                            // Удаляем клиента из контекста
                            DBConnection.massageSalon.Service.Remove(service);
                            DBConnection.massageSalon.SaveChanges();

                            Refresh();
                            MessageBox.Show("Услуга удалена.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        else
                        {
                            MessageBox.Show("Услуга не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                }
            }
            else if (result == MessageBoxResult.No) { }
        }

        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ServicesPage());
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (loggedWorker.Position.Name == "Администратор")
            {
                if (SearchTB.Text.Length > 0)

                    ServicesLV.ItemsSource = DBConnection.massageSalon.Service.Where(i => i.ID_TypeOfService == contextType.ID && i.Name.ToLower().StartsWith(SearchTB.Text.Trim().ToLower())).ToList();

                else
                    Refresh();
            }
            else if (loggedWorker.Position.Name == "Массажист")
            {
                if (SearchTB.Text.Length > 0)

                    ServicesForMassagistLV.ItemsSource = DBConnection.massageSalon.Service.Where(i => i.ID_TypeOfService == contextType.ID && i.Name.ToLower().StartsWith(SearchTB.Text.Trim().ToLower())).ToList();

                else
                    Refresh1();
            } 
        }
    }
}

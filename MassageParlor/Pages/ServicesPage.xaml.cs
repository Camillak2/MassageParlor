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
    /// Логика взаимодействия для ServicesPage.xaml
    /// </summary>
    public partial class ServicesPage : Page
    {
        Worker loggedWorker;

        public static List<TypeOfService> typeOfServices { get; set; }
        public ServicesPage()
        {
            InitializeComponent();
            loggedWorker = DBConnection.loginedWorker;
            CheckConditionAndToggleButtonVisibility();
            Refresh();
            this.DataContext = this;
        }

        public void Refresh()
        {
            TypesLV.ItemsSource = DBConnection.massageSalon.TypeOfService.ToList();
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

        private void TypesLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypesLV.SelectedItem is TypeOfService typeOfService)
            {
                TypesLV.SelectedItem = null;
                NavigationService.Navigate(new AllServicesPage(typeOfService));
            }
        }

        private void AddBTN_Click(object sender, RoutedEventArgs e)
        {
            AddTypeWindow addTypeWindow = new AddTypeWindow();
            addTypeWindow.ShowDialog();
            Refresh();
        }

        private void EditBTN_Click(object sender, RoutedEventArgs e)
        {
            if (TypesLV.SelectedItem is TypeOfService type)
            {
                DBConnection.selectedForEditType = TypesLV.SelectedItem as TypeOfService;
                EditTypeWindow editTypeWindow = new EditTypeWindow(type);
                editTypeWindow.ShowDialog();
            }
            else if (TypesLV.SelectedItem is null)
            {
                MessageBox.Show("Выберите тип услуги!");
            }
            Refresh();
        }
    }
}

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
    /// Логика взаимодействия для MainMenuPage.xaml
    /// </summary>
    public partial class MainMenuPage : Page
    {
        public static List<Worker> workers { get; set; }
        Worker loggedWorker;

        public MainMenuPage()
        {
            InitializeComponent();
            loggedWorker = DBConnection.loginedWorker;
            NameTB.Text = "Добро пожаловать, " + DBConnection.loginedWorker.Name.ToString() + "!";
            CheckConditionAndToggleButtonVisibility();
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
                AppealsBTN.Visibility = Visibility.Visible;
                //Не видно
                MassageBTN.Visibility = Visibility.Collapsed;
                ConnectionBTN.Visibility = Visibility.Collapsed;
            }
            else if (loggedWorker.Position.Name == "Массажист")
            {
                //Видно
                ProfileBTN.Visibility = Visibility.Visible;
                RecordsBTN.Visibility = Visibility.Visible;
                ServicesBTN.Visibility = Visibility.Visible;
                MassageBTN.Visibility = Visibility.Visible;
                LogOutBTN.Visibility = Visibility.Visible;
                ConnectionBTN.Visibility = Visibility.Visible;
                //Не видно
                WorkersBTN.Visibility = Visibility.Collapsed;
                ClientsBTN.Visibility = Visibility.Collapsed;
                AppealsBTN.Visibility= Visibility.Collapsed;
            }
        }

        private void ConnectionBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AppealsPage());
        }

        private void AppealsBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AppealsPage());
        }
    }
}

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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MassageParlor.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminMainMenuPage.xaml
    /// </summary>
    public partial class AdminMainMenuPage : Page
    {
        public static List<Worker> workers { get; set; }
        Worker loggedWorker;

        public AdminMainMenuPage()
        {
            InitializeComponent();
            loggedWorker = DBConnection.loginedWorker;
            NameTB.Text = "Добро пожаловать, " + DBConnection.loginedWorker.Surname.ToString() + " " + DBConnection.loginedWorker.Name.ToString() + " " + DBConnection.loginedWorker.Patronymic.ToString() + "!";
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

        private void LogOutBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthorizationPage());
        }

        private void CheckConditionAndToggleButtonVisibility()
        {
            if (loggedWorker.ID_Position == 1)
            {
                ClientsBTN.Visibility = Visibility.Visible; // Показать кнопку
            }
            else
            {
                ClientsBTN.Visibility = Visibility.Collapsed; // Скрыть кнопку
            }
        }
    }
}

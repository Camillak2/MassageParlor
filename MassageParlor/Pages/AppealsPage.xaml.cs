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
using MassageParlor.DB;

namespace MassageParlor.Pages
{
    /// <summary>
    /// Логика взаимодействия для AppealsPage.xaml
    /// </summary>
    public partial class AppealsPage : Page
    {
        Worker loggedWorker;
        public static List<Worker> workers { get; set; }
        public static List<Status> statuses { get; set; }
        public static List<Appeals> appeals { get; set; }

        public AppealsPage()
        {
            workers = DBConnection.massageSalon.Worker.ToList();
            statuses = DBConnection.massageSalon.Status.ToList();
            appeals = DBConnection.massageSalon.Appeals.ToList();

            InitializeComponent();
            loggedWorker = DBConnection.loginedWorker;
            if (loggedWorker.Position.Name == "Администратор")
            {
                NameTB.Text = "Обращения";
            }
            if (loggedWorker.Position.Name == "Массажист")
            {
                NameTB.Text = "Мои обращения";
            }
        }

        public void Refresh()
        {
            AppealsLV.ItemsSource = DBConnection.massageSalon.Appeals.ToList();
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
            AddAppealWindow addAppealWindow = new AddAppealWindow();
            addAppealWindow.ShowDialog();
            Refresh();
        }

        private void EditBTN_Click(object sender, RoutedEventArgs e)
        {
            if (AppealsLV.SelectedItem is Appeals appeals)
            {
                DBConnection.selectedForEditAppeal = AppealsLV.SelectedItem as Appeals;
                EditAppealWindow editAppealWindow = new EditAppealWindow(appeals);
                editAppealWindow.ShowDialog();
            }
            else if (AppealsLV.SelectedItem is null)
            {
                MessageBox.Show("Выберите обращение!");
            }
            Refresh();
        }

        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainMenuPage());
        }
    }
}

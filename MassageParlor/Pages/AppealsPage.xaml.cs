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
            InitializeComponent();
            loggedWorker = DBConnection.loginedWorker;
            workers = DBConnection.massageSalon.Worker.ToList();
            statuses = DBConnection.massageSalon.Status.ToList();
            statuses.Insert(0, new Status() { Name = "Все" });
            StatusCB.SelectedIndex = 0;

            appeals = DBConnection.massageSalon.Appeals.ToList();
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
                AddBTN.Visibility = Visibility.Collapsed;

                NameTB.Text = "Обращения";
            }
            else if (loggedWorker.Position.Name == "Массажист")
            {
                //Видно
                ProfileBTN.Visibility = Visibility.Visible;
                RecordsBTN.Visibility = Visibility.Visible;
                ServicesBTN.Visibility = Visibility.Visible;
                MassageBTN.Visibility = Visibility.Visible;
                AddBTN.Visibility = Visibility.Visible;
                LogOutBTN.Visibility = Visibility.Visible;

                //Не видно
                WorkersBTN.Visibility = Visibility.Collapsed;
                ClientsBTN.Visibility = Visibility.Collapsed;

                NameTB.Text = "Мои обращения";
            }
        }

        public void Refresh()
        {
            if (loggedWorker.Position.Name == "Администратор")
            {
                AppealsLV.ItemsSource = DBConnection.massageSalon.Appeals.ToList();
            }
            if (loggedWorker.Position.Name == "Массажист")
            {
                AppealsLV.ItemsSource = DBConnection.massageSalon.Appeals.Where(i => i.ID_Worker == loggedWorker.ID).ToList();
            }
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

        private void AppealsLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (loggedWorker.Position.Name == "Администратор")
            {
                if (AppealsLV.SelectedItem is Appeals appeals)
                {
                    DBConnection.selectedForEditAppeal = AppealsLV.SelectedItem as Appeals;
                    EditStatusWindow editStatusWindow = new EditStatusWindow(appeals);
                    editStatusWindow.ShowDialog();
                }
                else if (AppealsLV.SelectedItem is null)
                {
                    MessageBox.Show("Выберите обращение!");
                }
                Refresh();
            }
            else if (loggedWorker.Position.Name == "Массажист")
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
        }

        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainMenuPage());
        }

        private void StatusCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StatusCB.SelectedIndex == 0)
            {
                AppealsLV.ItemsSource = DBConnection.massageSalon.Appeals.ToList();
            }
            else if (StatusCB.SelectedIndex == 1)
            {
                AppealsLV.ItemsSource = DBConnection.massageSalon.Appeals.Where(i => i.Status.Name == "Не выполнено").ToList();
            }
            else if (StatusCB.SelectedIndex == 2)
            {
                AppealsLV.ItemsSource = DBConnection.massageSalon.Appeals.Where(i => i.Status.Name == "Выполнено").ToList();
            }
        }
    }
}

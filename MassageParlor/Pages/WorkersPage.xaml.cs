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
using MassageParlor.Windowww;

namespace MassageParlor.Pages
{
    /// <summary>
    /// Логика взаимодействия для WorkersPage.xaml
    /// </summary>
    public partial class WorkersPage : Page
    {
        Worker loggedWorker;
        public static List<Worker> workers {  get; set; }
        public static List<Position> positions { get; set; }
        public static List<Gender> genders { get; set; }

        public WorkersPage()
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
            WorkersLV.ItemsSource = DBConnection.massageSalon.Worker.ToList();
        }

        private bool IsWorkerExists(Worker worker)
        {
            return DBConnection.massageSalon.Record.Any(c => c.ID_Worker == worker.ID);
        }

        private void DeleteHL_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            var worker = (sender as Hyperlink).DataContext as Worker;
            if (result == MessageBoxResult.Yes)
            {
                if (IsWorkerExists(worker))
                {
                    MessageBox.Show("Этот сотрудник не может быть удален. Он участвует в записи.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    using (var db = new MassageSalonEntities())
                    {
                        // Находим клиента по его ID
                        var serviceToDelete = DBConnection.massageSalon.Worker.Find(worker.ID);

                        if (serviceToDelete != null)
                        {
                            // Удаляем клиента из контекста
                            DBConnection.massageSalon.Worker.Remove(worker);
                            DBConnection.massageSalon.SaveChanges();

                            Refresh();
                            MessageBox.Show("Сотрудник удален.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        else
                        {
                            MessageBox.Show("Сотрудник не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                }
            }
            else if (result == MessageBoxResult.No) { }
        }

        private void ProfileBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MyPersonalAccountPage());
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

        private void WorkersBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new WorkersPage());
        }

        private void MassageBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MassagePage());
        }

        private void LogOutBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthorizationPage());
        }

        private void AddBTN_Click(object sender, RoutedEventArgs e)
        {
            AddWorkerWindow addWorkerWindow = new AddWorkerWindow();
            addWorkerWindow.ShowDialog();
            Refresh();
        }

        private void WorkersLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WorkersLV.SelectedItem is Worker worker)
            {
                DBConnection.selectedForEditWorker = WorkersLV.SelectedItem as Worker;
                EditWorkerWindow editWorkerWindow = new EditWorkerWindow(worker);
                editWorkerWindow.ShowDialog();
            }
            else if (WorkersLV.SelectedItem is null)
            {
                MessageBox.Show("Выберите сотрудника!");
            }
            Refresh();
        }

        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainMenuPage());
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchTB.Text.Length > 0)

                WorkersLV.ItemsSource = DBConnection.massageSalon.Worker.Where(i => i.Surname.ToLower().StartsWith(SearchTB.Text.Trim().ToLower())
                || i.Name.ToLower().StartsWith(SearchTB.Text.Trim().ToLower()) || i.Patronymic.ToLower().StartsWith(SearchTB.Text.Trim().ToLower())).ToList();

            else
                WorkersLV.ItemsSource = DBConnection.massageSalon.Worker.Where(i => i.Position.Name == "Массажист").ToList();
        }

        private void WorkersChartBTN_Click(object sender, RoutedEventArgs e)
        {
            WorkersPopularityWindow workersPopularityWindow = new WorkersPopularityWindow();
            workersPopularityWindow.ShowDialog();
        }
    }
}

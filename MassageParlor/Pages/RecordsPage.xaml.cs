using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
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
using static MaterialDesignThemes.Wpf.Theme;

namespace MassageParlor.Pages
{
    /// <summary>
    /// Логика взаимодействия для RecordsPage.xaml
    /// </summary>
    public partial class RecordsPage : Page
    {
        //private CollectionViewSource _collectionViewSource;
        Worker loggedWorker;
        public static List<Record> records {  get; set; }
        public RecordsPage()
        {
            InitializeComponent();
            loggedWorker = DBConnection.loginedWorker;
            Refresh();
            records = DBConnection.massageSalon.Record.ToList();
            CheckConditionAndToggleButtonVisibility();
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DateDP.SelectedDate is null)
            {
                Refresh();
            }
            else
            {
                RecordsForAdminLV.ItemsSource = DBConnection.massageSalon.Record.Where(i => i.Date == DateDP.SelectedDate).ToList();
                RecordsForMassagistLV.ItemsSource = DBConnection.massageSalon.Record.Where(i => i.ID_Worker == loggedWorker.ID && i.Date == DateDP.SelectedDate).ToList();
            }
        }

        public void Refresh()
        {
            RecordsForAdminLV.ItemsSource = DBConnection.massageSalon.Record.Where(i => i.Date >= DateTime.Today).ToList();
            RecordsForMassagistLV.ItemsSource = DBConnection.massageSalon.Record.Where(i => i.ID_Worker == loggedWorker.ID && i.Date >= DateTime.Today).ToList();
        }

        public void Refresh2()
        {
            if (loggedWorker.Position.Name == "Администратор")
            {
                RecordsForAdminLV.ItemsSource = DBConnection.massageSalon.Record.ToList();
            }
            else if (loggedWorker.Position.Name == "Массажист")
            {
                RecordsForMassagistLV.ItemsSource = DBConnection.massageSalon.Record.Where(i => i.ID_Worker == loggedWorker.ID).ToList();
            }
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

                AddBTN.Visibility = Visibility.Visible;
                LogOutBTN.Visibility = Visibility.Visible;

                //Не видно
                MassageBTN.Visibility = Visibility.Collapsed;

                NameTB.Text = "Записи";
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
                AddBTN.Visibility = Visibility.Collapsed;
                ClientsBTN.Visibility = Visibility.Collapsed;

                NameTB.Text = "Мои записи";
            }
        }

        private void DeleteHL_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var record = (sender as Hyperlink).DataContext as Record;
                try
                {
                    DBConnection.massageSalon.Record.Remove(record);
                    DBConnection.massageSalon.SaveChanges();
                }
                catch
                {
                    MessageBox.Show("Эта запись не может быть удалена!");
                }

                Refresh();
            }
            else if (result == MessageBoxResult.No) { }
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
            AddRecordWindow addRecordWindow = new AddRecordWindow();
            addRecordWindow.ShowDialog();
            Refresh();
        }

        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainMenuPage());
        }

        private void AllCHB_Unchecked(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void AllCHB_Checked(object sender, RoutedEventArgs e)
        {
            DateDP.SelectedDate = null;
            Refresh2();
        }

        private void RecordsForAdminLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RecordsForAdminLV.SelectedItem is Record record)
            {
                DBConnection.selectedForEditRecord = RecordsForAdminLV.SelectedItem as Record;
                EditRecordWindow editRecordWindow = new EditRecordWindow(record);
                editRecordWindow.ShowDialog();
            }
            else if (RecordsForAdminLV.SelectedItem is null)
            {
                MessageBox.Show("Выберите запись!");
            }
            Refresh();
        }

        private void CleanBTN_Click(object sender, RoutedEventArgs e)
        {
            DateDP.SelectedDate = null;
            Refresh();
        }
    }
}

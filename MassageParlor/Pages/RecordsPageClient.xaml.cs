using MassageParlor.DB;
using MassageParlor.Windowww;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для RecordsPageClient.xaml
    /// </summary>
    public partial class RecordsPageClient : Page
    {
        public ObservableCollection<Record> Records { get; set; }
        Client loggedClient;
        public static List<Record> records { get; set; }

        public RecordsPageClient()
        {
            InitializeComponent();
            loggedClient = DBConnection.loginedClient;
            ActualRB.IsChecked = true;
            AllRB.IsChecked = false;
            LastRB.IsChecked = false;
            records = DBConnection.massageSalon.Record.ToList();
            Refresh();
            RefreshForMassagist();

            this.DataContext = this;
        }

        private void RefreshForMassagist()
        {
            records = DBConnection.massageSalon.Record.ToList();
            var sortedRecords = records.OrderBy(r => r.DateTime.Date).ThenBy(r => r.DateTime.TimeOfDay).ToList();
            Records = new ObservableCollection<Record>(sortedRecords);
            RecordsLV.ItemsSource = Records.Where(i => i.ID_Client == loggedClient.ID && i.DateTime > DateTime.Now && i.DateTime.Date == DateTime.Today);
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DateDP.SelectedDate is null)
            {
                Refresh();
            }
            else
            {
                ActualRB.IsChecked = false;
                AllRB.IsChecked = false;
                LastRB.IsChecked = false;
                records = DBConnection.massageSalon.Record.ToList();
                var sortedRecords = records.OrderBy(r => r.DateTime.Date).ThenBy(r => r.DateTime.TimeOfDay).ToList();
                Records = new ObservableCollection<Record>(sortedRecords);

                RecordsLV.ItemsSource = Records.Where(i => i.DateTime.Date == DateDP.SelectedDate);
            }
        }

        public void Refresh()
        {
            records = DBConnection.massageSalon.Record.ToList();
            var sortedRecords = records.OrderBy(r => r.DateTime.Date).ThenBy(r => r.DateTime.TimeOfDay).ToList();
            Records = new ObservableCollection<Record>(sortedRecords);

            RecordsLV.ItemsSource = Records.Where(i => i.DateTime >= DateTime.Now);

            ActualRB.IsChecked = true;
            AllRB.IsChecked = false;
            LastRB.IsChecked = false;
        }

        public void Refresh2()
        {
            records = DBConnection.massageSalon.Record.ToList();
            var sortedRecords = records.OrderBy(r => r.DateTime.Date).ThenBy(r => r.DateTime.TimeOfDay).ToList();
            Records = new ObservableCollection<Record>(sortedRecords);

            RecordsLV.ItemsSource = Records;

            ActualRB.IsChecked = false;
            AllRB.IsChecked = true;
            LastRB.IsChecked = false;
        }

        public void Refresh3()
        {
            records = DBConnection.massageSalon.Record.ToList();
            var sortedRecords = records.OrderBy(r => r.DateTime.Date).ThenBy(r => r.DateTime.TimeOfDay).ToList();
            Records = new ObservableCollection<Record>(sortedRecords);

            RecordsLV.ItemsSource = Records.Where(i => i.DateTime < DateTime.Now);

            ActualRB.IsChecked = false;
            AllRB.IsChecked = false;
            LastRB.IsChecked = true;
        }

        private void DeleteHL_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            var lastTime = records.Select(x => x.DateTime.TimeOfDay);


            if (result == MessageBoxResult.Yes)
            {
                var record = (sender as Hyperlink).DataContext as Record;
                if (record.DateTime > DateTime.Now)
                {
                    DBConnection.massageSalon.Record.Remove(record);
                    DBConnection.massageSalon.SaveChanges();
                }
                else
                {
                    MessageBox.Show("Эта запись не может быть удалена.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                Refresh();
            }
            else if (result == MessageBoxResult.No) { }
        }

        private void ProfileBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MyAccountPage());
        }

        private void RecordsBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RecordsPageClient());
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

        private void CleanBTN_Click(object sender, RoutedEventArgs e)
        {
            ActualRB.IsChecked = true;
            DateDP.SelectedDate = null;
            Refresh();
        }

        private void ActualCHB_Checked(object sender, RoutedEventArgs e)
        {
            DateDP.SelectedDate = null;
            Refresh();
            AllRB.IsChecked = false;
            LastRB.IsChecked = false;
        }

        private void AllCHB_Checked(object sender, RoutedEventArgs e)
        {
            DateDP.SelectedDate = null;
            Refresh2();
            ActualRB.IsChecked = false;
            LastRB.IsChecked = false;
        }

        private void LastRB_Checked(object sender, RoutedEventArgs e)
        {
            DateDP.SelectedDate = null;
            Refresh3();
            ActualRB.IsChecked = false;
            AllRB.IsChecked = false;
        }

        private void ChartBTN_Click(object sender, RoutedEventArgs e)
        {
            StatisticWindow statisticWindow = new StatisticWindow();
            statisticWindow.ShowDialog();
        }
    }
}

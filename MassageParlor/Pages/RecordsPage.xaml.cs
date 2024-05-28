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
        public ObservableCollection<Record> Records { get; set; }
        Worker loggedWorker;
        public static List<Record> records {  get; set; }
        public RecordsPage()
        {
            InitializeComponent();
            loggedWorker = DBConnection.loginedWorker;
            ActualRB.IsChecked = true;
            AllRB.IsChecked = false;
            LastRB.IsChecked = false;
            records = DBConnection.massageSalon.Record.ToList();
            Refresh();
            RefreshSumPrice();
            CheckConditionAndToggleButtonVisibility();
            this.DataContext = this;
        }

        private void RefreshSumPrice()
        {
            decimal result = 0;
            foreach (var i in records)
            {
                if (i.DateTime < DateTime.Now)
                {
                    decimal c = Convert.ToDecimal(i.FinalPrice);
                    result += c;
                }
                else
                {
                    result += 0;
                }
            }
            SumPriceTB.Text = result.ToString() + "₽";
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

                RecordsForAdminLV.ItemsSource = Records.Where(i => i.DateTime.Date == DateDP.SelectedDate);
                RecordsForMassagistLV.ItemsSource = Records.Where(i => i.ID_Worker == loggedWorker.ID && i.DateTime == DateDP.SelectedDate);
            }
        }

        public void Refresh()
        {
            records = DBConnection.massageSalon.Record.ToList();
            var sortedRecords = records.OrderBy(r => r.DateTime.Date).ThenBy(r => r.DateTime.TimeOfDay).ToList();
            Records = new ObservableCollection<Record>(sortedRecords);

            RecordsForAdminLV.ItemsSource = Records.Where(i => i.DateTime >= DateTime.Now);
            RecordsForMassagistLV.ItemsSource = Records.Where(i => i.ID_Worker == loggedWorker.ID && i.DateTime >= DateTime.Now);
        }

        public void Refresh2()
        {
            records = DBConnection.massageSalon.Record.ToList();
            var sortedRecords = records.OrderBy(r => r.DateTime.Date).ThenBy(r => r.DateTime.TimeOfDay).ToList();
            Records = new ObservableCollection<Record>(sortedRecords);

            RecordsForAdminLV.ItemsSource = Records;
            RecordsForMassagistLV.ItemsSource = Records.Where(i => i.ID_Worker == loggedWorker.ID);
        }

        public void Refresh3()
        {
            records = DBConnection.massageSalon.Record.ToList();
            var sortedRecords = records.OrderBy(r => r.DateTime.Date).ThenBy(r => r.DateTime.TimeOfDay).ToList();
            Records = new ObservableCollection<Record>(sortedRecords);

            RecordsForAdminLV.ItemsSource = Records.Where(i => i.DateTime < DateTime.Now);
            RecordsForMassagistLV.ItemsSource = Records.Where(i => i.ID_Worker == loggedWorker.ID && i.DateTime < DateTime.Now);
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
                RecordsForAdminLV.Visibility = Visibility.Visible;
                ForAdmin.Visibility = Visibility.Visible;

                //Не видно
                MassageBTN.Visibility = Visibility.Collapsed;
                RecordsForMassagistLV.Visibility = Visibility.Collapsed;

                NameTB.Text = "Записи";
            }
            else if (loggedWorker.Position.Name == "Массажист")
            {
                //Видно
                ProfileBTN.Visibility = Visibility.Visible;
                RecordsBTN.Visibility = Visibility.Visible;
                ServicesBTN.Visibility = Visibility.Visible;
                MassageBTN.Visibility = Visibility.Visible;
                RecordsForMassagistLV.Visibility= Visibility.Visible;
                LogOutBTN.Visibility = Visibility.Visible;

                //Не видно
                WorkersBTN.Visibility = Visibility.Collapsed;
                AddBTN.Visibility = Visibility.Collapsed;
                ClientsBTN.Visibility = Visibility.Collapsed;
                RecordsForAdminLV.Visibility= Visibility.Collapsed;
                ForAdmin.Visibility= Visibility.Collapsed;

                NameTB.Text = "Мои записи";
            }
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

        //private void RecordsForAdminLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (RecordsForAdminLV.SelectedItem is Record record)
        //    {
        //        if (record.DateTime < DateTime.Now)
        //        {
        //            MessageBox.Show("Эту запись нельзя изменить.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
        //            return;
        //        }
        //        else
        //        {
        //            DBConnection.selectedForEditRecord = RecordsForAdminLV.SelectedItem as Record;
        //            EditRecordWindow editRecordWindow = new EditRecordWindow(record);
        //            editRecordWindow.ShowDialog();
        //        }
        //    }
        //    else if (RecordsForAdminLV.SelectedItem is null)
        //    {
        //        MessageBox.Show("Выберите запись.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
        //        return;
        //    }
        //    Refresh();
        //    //MessageBox.Show("Запись изменить нельзя. Удалите запись и создайте новую.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
        //    //return;
        //}

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
            statisticWindow.Show();
        }
    }
}

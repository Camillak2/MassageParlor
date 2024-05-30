using LiveCharts.Wpf;
using MassageParlor.DB;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static MaterialDesignThemes.Wpf.Theme;

namespace MassageParlor.Windowww
{
    /// <summary>
    /// Логика взаимодействия для AddRecordWindow.xaml
    /// </summary>
    public partial class AddRecordWindow : Window
    {
        public static List<Worker> workers { get; set; }
        public static List<Client> clients { get; set; }
        public static List<Gender> genders { get; set; }
        public static List<Service> services { get; set; }
        public static List<Discount> discounts { get; set; }
        public static List<TypeOfService> types { get; set; }
        public static List<Record> records { get; set; }
        public static Record record = new Record();

        public AddRecordWindow()
        {
            InitializeComponent();
            workers = DBConnection.massageSalon.Worker.ToList();
            clients = DBConnection.massageSalon.Client.ToList();
            services = DBConnection.massageSalon.Service.ToList();
            types = DBConnection.massageSalon.TypeOfService.ToList();
            discounts = DBConnection.massageSalon.Discount.ToList();
            records = DBConnection.massageSalon.Record.ToList();
            DiscountCB.SelectedIndex = 0;
            DateDP.IsEnabled = false;
            //TimeCB.IsEnabled = false;
            Refresh();

            this.DataContext = this;

            DateDP.SelectedDateChanged += DatePicker_SelectedDateChanged;
            DateDP.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, DateTime.Now.Date.AddDays(-1)));
        }

        public void Refresh()
        {
            WorkersLV.ItemsSource = DBConnection.massageSalon.Worker.Where(i => i.Position.Name == "Массажист").ToList();
            ClientsLV.ItemsSource = DBConnection.massageSalon.Client.ToList();
            ServicesLV.ItemsSource = DBConnection.massageSalon.Service.ToList();
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DateDP.SelectedDate < DateTime.Now.Date)
            {
                MessageBox.Show("Нельзя выбрать прошедшую дату.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                DateDP.SelectedDate = null;
            }
            else
            {
                UpdateAvailableTimes();
            }

            UpdateAvailableTimes();
        }

        private void TimeLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAvailableTimes();
        }

        private void UpdateAvailableTimes()
        {
            var selectedWorker = WorkersLV.SelectedItem as Worker;
            var selectedDate = DateDP.SelectedDate;
            if (selectedWorker == null || selectedDate == null)
            {
                //TimeCB.ItemsSource = null;
                TimeLV.ItemsSource = null;
                return;
            }

            var unavailableTimes = new List<TimeSpan>();

            var workerRecords = records.Where(r => r.ID_Worker == selectedWorker.ID && r.DateTime.Date == selectedDate.Value.Date);

            foreach (var record in workerRecords)
            {
                var service = services.FirstOrDefault(s => s.ID == record.ID_Service);
                if (service != null)
                {
                    TimeSpan start = record.DateTime.TimeOfDay;
                    TimeSpan end = start.Add(service.Duration.Value).Add(TimeSpan.FromMinutes(15)); // добавляем 15 минут на перерыв

                    for (TimeSpan time = start; time <= end; time = time.Add(TimeSpan.FromMinutes(15)))
                    {
                        unavailableTimes.Add(time);
                    }

                    start = record.DateTime.TimeOfDay.Subtract(service.Duration.Value).Subtract(TimeSpan.FromMinutes(15));
                    end = record.DateTime.TimeOfDay;
                    for (TimeSpan time = start; time <= end; time = time.Add(TimeSpan.FromMinutes(15)))
                    {
                        unavailableTimes.Add(time);
                    }
                }
            }

            //var availableTimes1 = new List<TimeSpan>();
            //var now = DateTime.Now;
            //bool isToday = selectedDate.Value.Date == now.Date;
            //TimeSpan startTime = isToday ? now.TimeOfDay : new TimeSpan(8, 0, 0);

            //for (TimeSpan time = startTime; time < new TimeSpan(20, 15, 0); time = time.Add(TimeSpan.FromMinutes(15)))
            //{
            //    if (!unavailableTimes.Contains(time))
            //    {
            //        availableTimes1.Add(time);
            //    }
            //}
            //TimeLV.ItemsSource = availableTimes1.Select(t => t.ToString(@"hh\:mm")).ToList();

            var availableTimes1 = new List<TimeSpan>();
            var now = DateTime.Now;
            bool isToday = selectedDate.Value.Date == now.Date;
            TimeSpan startTime = isToday ? now.TimeOfDay : new TimeSpan(8, 0, 0);

            startTime = new TimeSpan(startTime.Hours, (startTime.Minutes / 15) * 30, 0);

            TimeSpan endTime = new TimeSpan(20, 15, 0);

            for (TimeSpan time = startTime; time < endTime; time = time.Add(TimeSpan.FromMinutes(15)))
            {
                if (!unavailableTimes.Contains(time))
                {
                    availableTimes1.Add(time);
                }
            }

            TimeLV.ItemsSource = availableTimes1.Select(t => t.ToString(@"hh\:mm")).ToList();

            //var availableTimes = new List<TimeSpan>();
            //for (TimeSpan time = new TimeSpan(8, 0, 0); time < new TimeSpan(20, 15, 0); time = time.Add(TimeSpan.FromMinutes(15)))
            //{
            //    if (!unavailableTimes.Contains(time))
            //    {
            //        availableTimes.Add(time);
            //    }
            //}
            ////TimeCB.ItemsSource = availableTimes;
            //TimeLV.ItemsSource = availableTimes.Select(t => t.ToString(@"hh\:mm")).ToList();
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedClient = ClientsLV.SelectedItem as Client;
                var selectedWorker = WorkersLV.SelectedItem as Worker;
                var selectedService = ServicesLV.SelectedItem as Service;
                var selectedDiscount = DiscountCB.SelectedItem as Discount;
                var selectedDate = DateDP.SelectedDate;
                var selectedTimeString = TimeLV.SelectedItem as string;
                //var selectedTime = TimeCB.SelectedItem as TimeSpan?;

                if (selectedClient == null ||
                    selectedWorker == null ||
                    selectedService == null ||
                    selectedDiscount == null ||
                    selectedDate == null ||
                    //selectedTime == null)
                    string.IsNullOrEmpty(selectedTimeString))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.");
                    return;
                }


                TimeSpan selectedTime = TimeSpan.Parse(selectedTimeString);
                //DateTime appointmentDateTime = selectedDate.Value.Date + selectedTime.Value;
                DateTime appointmentDateTime = selectedDate.Value.Date + selectedTime;

                if (IsWorkerAvailable(selectedWorker.ID, appointmentDateTime, selectedService.Duration.Value))
                {
                    Record newRecord = new Record
                    {
                        ID_Client = selectedClient.ID,
                        ID_Worker = selectedWorker.ID,
                        ID_Service = selectedService.ID,
                        DateTime = appointmentDateTime,
                        ID_Discount = selectedDiscount.ID,
                        FinalPrice = decimal.Parse(FinalPriceTB.Text)
                    };

                    DBConnection.massageSalon.Record.Add(newRecord);
                    DBConnection.massageSalon.SaveChanges();
                    MessageBox.Show("Запись успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.None);
                    UpdateAvailableTimes();
                    Close();
                }
                else
                {
                    MessageBox.Show("Выбранное время занято.");
                }
            }
            catch
            {
                MessageBox.Show("Непредвиденная ошибка.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private bool IsWorkerAvailable(int workerId, DateTime appointmentDateTime, TimeSpan serviceDuration)
        {
            var workerRecords = records.Where(r => r.ID_Worker == workerId && r.DateTime.Date == appointmentDateTime.Date);

            foreach (var record in workerRecords)
            {
                var service = services.FirstOrDefault(s => s.ID == record.ID_Service);
                if (service != null)
                {
                    DateTime start = record.DateTime;
                    DateTime end = start.Add(service.Duration.Value).Add(TimeSpan.FromMinutes(15));

                    if (appointmentDateTime >= start && appointmentDateTime < end)
                    {
                        return false;
                    }

                    start = record.DateTime.Subtract(service.Duration.Value).Subtract(TimeSpan.FromMinutes(15));
                    end = record.DateTime;
                    if (appointmentDateTime >= start && appointmentDateTime < end)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void ChooseMassagistBTN_Click(object sender, RoutedEventArgs e)
        {
            Grid1.Visibility = Visibility.Collapsed;
            SaveBTN.Visibility = Visibility.Collapsed;
            CancelBTN.Visibility = Visibility.Collapsed;
            Grid2.Visibility = Visibility.Visible;
            Grid3.Visibility = Visibility.Collapsed;
            Grid4.Visibility = Visibility.Collapsed;
        }

        private void ChooseClientBTN_Click(object sender, RoutedEventArgs e)
        {
            Grid1.Visibility = Visibility.Collapsed;
            SaveBTN.Visibility = Visibility.Collapsed;
            CancelBTN.Visibility = Visibility.Collapsed;
            Grid2.Visibility = Visibility.Collapsed;
            Grid3.Visibility = Visibility.Visible;
            Grid4.Visibility = Visibility.Collapsed;
        }

        private void ChooseServiceBTN_Click(object sender, RoutedEventArgs e)
        {
            Grid1.Visibility = Visibility.Collapsed;
            SaveBTN.Visibility = Visibility.Collapsed;
            CancelBTN.Visibility = Visibility.Collapsed;
            Grid2.Visibility = Visibility.Collapsed;
            Grid3.Visibility = Visibility.Collapsed;
            Grid4.Visibility = Visibility.Visible;
        }

        private void WorkersLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WorkersLV.SelectedItem != null)
            {
                dynamic selectedItem = WorkersLV.SelectedItem;

                MassagistTB.Text = selectedItem.Surname + " " + selectedItem.Name + " " + selectedItem.Patronymic;

                Grid1.Visibility = Visibility.Visible;
                SaveBTN.Visibility = Visibility.Visible;
                CancelBTN.Visibility = Visibility.Visible;
                Grid2.Visibility = Visibility.Collapsed;
                Grid3.Visibility = Visibility.Collapsed;
                Grid4.Visibility = Visibility.Collapsed;

                DateDP.IsEnabled = true;
                //TimeCB.IsEnabled = true;

                UpdateAvailableTimes();
            }
            else
            {
                MassagistTB.Text = "";
                DateDP.IsEnabled = false;
            }
        }

        private void Close1BTN_Click(object sender, RoutedEventArgs e)
        {
            Grid1.Visibility = Visibility.Visible;
            SaveBTN.Visibility = Visibility.Visible;
            CancelBTN.Visibility = Visibility.Visible;
            Grid2.Visibility = Visibility.Collapsed;
            Grid3.Visibility = Visibility.Collapsed;
            Grid4.Visibility = Visibility.Collapsed;
        }

        private void ClientsLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic selectedClient = ClientsLV.SelectedItem;
            int clientCount = records.Where(i => i.DateTime < DateTime.Now).Count(r => r.ID_Client == selectedClient.ID);
            dynamic selectedService = ServicesLV.SelectedItem;

            ClientTB.Text = selectedClient.Surname + " " + selectedClient.Name + " " + selectedClient.Patronymic;
            if (selectedService != null)
            {
                ServiceTB.Text = selectedService.Name;
                if (records.Any(i => i.ID_Client == selectedClient.ID))
                {
                    DiscountCB.SelectedIndex = 0;
                    PriceServiceTB.Text = selectedService.Price.ToString();
                    FinalPriceTB.Text = PriceServiceTB.Text;
                }
                else if (records.Any(i => i.ID_Client == selectedClient.ID) && clientCount >= 5)
                {
                    DiscountCB.SelectedIndex = 4;
                    PriceServiceTB.Text = selectedService.Price.ToString();
                    FinalPriceTB.Text = (selectedService.Price - selectedService.Price / 100 * 10).ToString();
                }
                else
                {
                    DiscountCB.SelectedIndex = 1;
                    PriceServiceTB.Text = selectedService.Price.ToString();
                    FinalPriceTB.Text = PriceServiceTB.Text;
                }
            }
            else
            {
                ServiceTB.Text = "";
            }

            
            Grid1.Visibility = Visibility.Visible;
            SaveBTN.Visibility = Visibility.Visible;
            CancelBTN.Visibility = Visibility.Visible;
            Grid2.Visibility = Visibility.Collapsed;
            Grid3.Visibility = Visibility.Collapsed;
            Grid4.Visibility = Visibility.Collapsed;
        }

        private void Close2BTN_Click(object sender, RoutedEventArgs e)
        {
            Grid1.Visibility = Visibility.Visible;
            SaveBTN.Visibility = Visibility.Visible;
            CancelBTN.Visibility = Visibility.Visible;
            Grid2.Visibility = Visibility.Collapsed;
            Grid3.Visibility = Visibility.Collapsed;
            Grid4.Visibility = Visibility.Collapsed;
        }

        private void ServicesLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic selectedService = ServicesLV.SelectedItem;

            if (selectedService.TypeOfService.Name == "SPA для лица" || selectedService.Name == "Волшебное прикосновение")
            {
                DiscountCB.SelectedIndex = 3;
                ServiceTB.Text = selectedService.Name;
                PriceServiceTB.Text = selectedService.Price.ToString();
                FinalPriceTB.Text = (selectedService.Price - selectedService.Price / 100 * 20).ToString();
            }
            else
            {
                DiscountCB.SelectedIndex = 0;
                ServiceTB.Text = selectedService.Name;
                PriceServiceTB.Text = selectedService.Price.ToString();
                FinalPriceTB.Text = PriceServiceTB.Text;
            }

            Grid1.Visibility = Visibility.Visible;
            SaveBTN.Visibility = Visibility.Visible;
            CancelBTN.Visibility = Visibility.Visible;
            Grid2.Visibility = Visibility.Collapsed;
            Grid3.Visibility = Visibility.Collapsed;
            Grid4.Visibility = Visibility.Collapsed;
        }

        private void Close3BTN_Click(object sender, RoutedEventArgs e)
        {
            Grid1.Visibility = Visibility.Visible;
            SaveBTN.Visibility = Visibility.Visible;
            CancelBTN.Visibility = Visibility.Visible;
            Grid2.Visibility = Visibility.Collapsed;
            Grid3.Visibility = Visibility.Collapsed;
            Grid4.Visibility = Visibility.Collapsed;
        }

        private bool IsDigit(string text)
        {
            return Regex.IsMatch(text, "[0-9]");
        }

        private void Search1TB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Search1TB.Text.Length > 0)

                WorkersLV.ItemsSource = DBConnection.massageSalon.Worker.Where(i => i.Position.Name == "Массажист" && i.Surname.ToLower().StartsWith(Search1TB.Text.Trim().ToLower())
                || i.Position.Name == "Массажист" && i.Name.ToLower().StartsWith(Search1TB.Text.Trim().ToLower()) || i.Position.Name == "Массажист" && i.Patronymic.ToLower().StartsWith(Search1TB.Text.Trim().ToLower())).ToList();

            else
                WorkersLV.ItemsSource = DBConnection.massageSalon.Worker.Where(i => i.Position.Name == "Массажист").ToList();
        }

        private void Search2TB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Search2TB.Text.Length > 0)

                ClientsLV.ItemsSource = DBConnection.massageSalon.Client.Where(i => i.Surname.ToLower().StartsWith(Search2TB.Text.Trim().ToLower())
                || i.Name.ToLower().StartsWith(Search2TB.Text.Trim().ToLower()) || i.Patronymic.ToLower().StartsWith(Search2TB.Text.Trim().ToLower())).ToList();

            else
                ClientsLV.ItemsSource = DBConnection.massageSalon.Client.ToList();
        }

        private void Search3TB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Search3TB.Text.Length > 0)
                ServicesLV.ItemsSource = DBConnection.massageSalon.Service.Where(i => i.Name.ToLower().StartsWith(Search3TB.Text.Trim().ToLower())).ToList();
            else
                ServicesLV.ItemsSource = DBConnection.massageSalon.Service.ToList();
        }

        private void DiscountCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Discount selectedDiscount = (Discount)DiscountCB.SelectedItem;
            Client selectedClient = (Client)ClientsLV.SelectedItem;
            Service selectedService = (Service)ServicesLV.SelectedItem;
            
            if (ServicesLV.SelectedItem != null)
            {
                if (DiscountCB.SelectedIndex == 0)
                {
                    PriceServiceTB.Text = selectedService.Price.ToString();
                    FinalPriceTB.Text = PriceServiceTB.Text;
                    record.FinalPrice = Convert.ToDecimal(FinalPriceTB.Text.Trim());
                    record.ID_Discount = 1;
                }
                else
                {
                    if (DiscountCB.SelectedIndex == 1)
                    {
                        if(records.Any(i => i.ID_Client == selectedClient.ID))
                        {
                            MessageBox.Show($"Клиенту {selectedClient.Surname + " " + selectedClient.Name + " " + selectedClient.Patronymic} скидка «{selectedDiscount.Name}» недоступна.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
                            DiscountCB.SelectedIndex = 0;
                            PriceServiceTB.Text = selectedService.Price.ToString();
                            record.ID_Discount = 1;
                            return;
                        }
                        else
                        {
                            FinalPriceTB.Text = (selectedService.Price - selectedService.Price / 100 * 20).ToString();
                            record.FinalPrice = Convert.ToDecimal(FinalPriceTB.Text.Trim());
                            record.ID_Discount = 2;
                        }
                    }
                    else if (DiscountCB.SelectedIndex == 2)
                    {
                        if (Convert.ToDateTime(selectedClient.DateOfBirth).Day == DateTime.Now.Day)
                        {
                            FinalPriceTB.Text = (selectedService.Price - selectedService.Price / 100 * 30).ToString();
                            record.FinalPrice = Convert.ToDecimal(FinalPriceTB.Text.Trim());
                            record.ID_Discount = 3;
                        }
                        else
                        {
                            MessageBox.Show($"Клиенту {selectedClient.Surname + " " + selectedClient.Name + " " + selectedClient.Patronymic} скидка «{selectedDiscount.Name}» недоступна.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
                            DiscountCB.SelectedIndex = 0;
                            record.ID_Discount = 1;
                            return;
                        }
                    }
                    else if (DiscountCB.SelectedIndex == 3)
                    {
                        if (selectedService.TypeOfService.Name == "SPA для лица" || selectedService.Name == "Волшебное прикосновение")
                        {
                            FinalPriceTB.Text = (selectedService.Price - selectedService.Price / 100 * 20).ToString();
                            record.FinalPrice = Convert.ToDecimal(FinalPriceTB.Text.Trim());
                            record.ID_Discount = 4;
                        }
                        else
                        {
                            MessageBox.Show($"Скидка «{selectedDiscount.Name}» применяется только к программе массажа лица.", "Внимание", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                            DiscountCB.SelectedIndex = 0;
                            FinalPriceTB.Text = PriceServiceTB.Text;
                            record.FinalPrice = Convert.ToDecimal(FinalPriceTB.Text.Trim());
                            record.ID_Discount = 1;
                            return;
                        }
                    }
                    if (DiscountCB.SelectedIndex == 4 || DiscountCB.Text == "Постоянный клиент")
                    {
                        int clientCount = records.Where(i => i.DateTime < DateTime.Now).Count(r => r.ID_Client == selectedClient.ID);

                        if (clientCount >= 5)
                        {
                            FinalPriceTB.Text = (selectedService.Price - selectedService.Price / 100 * 20).ToString();
                            record.FinalPrice = Convert.ToDecimal(FinalPriceTB.Text.Trim());
                            record.ID_Discount = 5;
                        }
                        else
                        {
                            MessageBox.Show($"Клиенту {selectedClient.Surname + " " + selectedClient.Name + " " + selectedClient.Patronymic} скидка «{selectedDiscount.Name}» недоступна.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
                            DiscountCB.SelectedIndex = 0;
                            record.ID_Discount = 1;
                            return;
                        }
                    } 
                }
            }
        }

        private void CancelBTN_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddClientBTN_Click(object sender, RoutedEventArgs e)
        {
            AddClientWindow addClientWindow = new AddClientWindow();
            addClientWindow.ShowDialog();
            Refresh();
        }
    }
}

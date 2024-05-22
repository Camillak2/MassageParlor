using MassageParlor.DB;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MassageParlor.Windowww
{
    /// <summary>
    /// Логика взаимодействия для EditRecordWindow.xaml
    /// </summary>
    public partial class EditRecordWindow : Window
    {
        public static List<Worker> workers { get; set; }
        public static List<Client> clients { get; set; }
        public static List<Gender> genders { get; set; }
        public static List<Service> services { get; set; }
        public static List<Discount> discounts { get; set; }
        public static List<TypeOfService> types { get; set; }
        public static List<Record> records { get; set; }

        Record contextRecord;

        public EditRecordWindow(Record record)
        {
            InitializeComponent();
            contextRecord = record;

            workers = DBConnection.massageSalon.Worker.ToList();
            clients = DBConnection.massageSalon.Client.ToList();
            genders = DBConnection.massageSalon.Gender.ToList();
            services = DBConnection.massageSalon.Service.ToList();
            types = DBConnection.massageSalon.TypeOfService.ToList();
            discounts = DBConnection.massageSalon.Discount.ToList();
            records = DBConnection.massageSalon.Record.ToList();
            Refresh();
            MassagistTB.Text = contextRecord.Worker.Surname + " " + contextRecord.Worker.Name + " " + contextRecord.Worker.Patronymic;
            ClientTB.Text = contextRecord.Client.Surname + " " + contextRecord.Client.Name + " " + contextRecord.Client.Patronymic;
            ServiceTB.Text = contextRecord.Service.Name;
            PriceServiceTB.Text = contextRecord.Service.Price.ToString();
            DiscountCB.SelectedIndex = (int)contextRecord.ID_Discount - 1;
            FinalPriceTB.Text = contextRecord.FinalPrice.ToString();
            DateDP.SelectedDate = contextRecord.DateTime.Date;
            TimeTP.SelectedTime = contextRecord.DateTime.Date.Add(contextRecord.DateTime.TimeOfDay);
            TimeTB.Text = TimeTP.ToString();

            DateDP.SelectedDateChanged += DatePicker_SelectedDateChanged;
            DateDP.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, DateTime.Now.Date.AddDays(-1)));
            this.DataContext = this;
        }

        public void Refresh()
        {
            WorkersLV.ItemsSource = DBConnection.massageSalon.Worker.Where(i => i.Position.Name == "Массажист").ToList();
            ClientsLV.ItemsSource = DBConnection.massageSalon.Client.ToList();
            ServicesLV.ItemsSource = DBConnection.massageSalon.Service.ToList();
        }

        private void EditBTN_Click(object sender, RoutedEventArgs e)
        {
            ChooseMassagistBTN.IsEnabled = true;
            ChooseClientBTN.IsEnabled = true;
            ChooseServiceBTN.IsEnabled = true;
            DiscountCB.IsEnabled = true;
            DateDP.IsEnabled = true;
            TimeTP.IsEnabled = true;


            SaveBTN.Visibility = Visibility.Visible;
            EditBTN.Visibility = Visibility.Collapsed;
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Record record = contextRecord;
                if (string.IsNullOrWhiteSpace(MassagistTB.Text) || string.IsNullOrWhiteSpace(ServiceTB.Text) || string.IsNullOrWhiteSpace(ClientTB.Text) ||
                        DateDP.SelectedDate == null || TimeTP.SelectedTime == null)
                {
                    MessageBox.Show("Заполните все поля.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                else
                {
                    if (WorkersLV.SelectedItem is Worker worker)
                    {
                        record.ID_Worker = worker.ID;
                    }
                    else if (ClientsLV.SelectedItem is Client client)
                    {
                        record.ID_Client = client.ID;
                    }
                    else if (ServicesLV.SelectedItem is Service service)
                    {
                        record.ID_Service = service.ID;
                    }
                    else
                    {
                        record.ID_Worker = contextRecord.ID_Worker;
                        record.ID_Client = contextRecord.ID_Client;
                        record.ID_Service = contextRecord.ID_Service;
                    }

                    if (DateDP.SelectedDate == null || TimeTP.SelectedTime == null)
                    {
                        if (!DateTime.TryParseExact(TimeTB.Text, "HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime timePart))
                        {
                            MessageBox.Show("Неверный формат времени. Используйте формат HH:mm.");
                            return;
                        }
                        DateTime dateTime = DateDP.SelectedDate.Value.Date.Add(timePart.TimeOfDay);
                    }
                    else
                    {
                        DateTime recordDateTime = DateDP.SelectedDate.Value.Date.Add(TimeTP.SelectedTime.Value.TimeOfDay);
                        record.DateTime = recordDateTime;
                    }

                    DBConnection.massageSalon.SaveChanges();
                    Close();
                }
            }
            catch
            {
                MessageBox.Show("Непредвиденная ошибка.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void ChooseMassagistBTN_Click(object sender, RoutedEventArgs e)
        {
            Grid1.Visibility = Visibility.Collapsed;
            SaveBTN.Visibility = Visibility.Collapsed;
            Grid2.Visibility = Visibility.Visible;
            Grid3.Visibility = Visibility.Collapsed;
            Grid4.Visibility = Visibility.Collapsed;
        }

        private void ChooseClientBTN_Click(object sender, RoutedEventArgs e)
        {
            Grid1.Visibility = Visibility.Collapsed;
            SaveBTN.Visibility = Visibility.Collapsed;
            Grid2.Visibility = Visibility.Collapsed;
            Grid3.Visibility = Visibility.Visible;
            Grid4.Visibility = Visibility.Collapsed;
        }

        private void ChooseServiceBTN_Click(object sender, RoutedEventArgs e)
        {
            Grid1.Visibility = Visibility.Collapsed;
            SaveBTN.Visibility = Visibility.Collapsed;
            Grid2.Visibility = Visibility.Collapsed;
            Grid3.Visibility = Visibility.Collapsed;
            Grid4.Visibility = Visibility.Visible;
        }

        private void WorkersLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WorkersLV.SelectedItem != null)
            {
                // Получаем выбранный объект
                dynamic selectedItem = WorkersLV.SelectedItem;

                // Отображаем ФИО в TextBox
                MassagistTB.Text = selectedItem.Surname + " " + selectedItem.Name + " " + selectedItem.Patronymic;

                Grid1.Visibility = Visibility.Visible;
                SaveBTN.Visibility = Visibility.Visible;
                Grid2.Visibility = Visibility.Collapsed;
                Grid3.Visibility = Visibility.Collapsed;
                Grid4.Visibility = Visibility.Collapsed;
            }
            else
            {
                MassagistTB.Text = contextRecord.Worker.Surname + " " + contextRecord.Worker.Name + " " + contextRecord.Worker.Patronymic;
            }
        }

        private void Close1BTN_Click(object sender, RoutedEventArgs e)
        {
            Grid1.Visibility = Visibility.Visible;
            SaveBTN.Visibility = Visibility.Visible;
            Grid2.Visibility = Visibility.Collapsed;
            Grid3.Visibility = Visibility.Collapsed;
            Grid4.Visibility = Visibility.Collapsed;
        }

        private void ClientsLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClientsLV.SelectedItem != null)
            {
                // Получаем выбранный объект
                dynamic selectedItem = ClientsLV.SelectedItem;

                // Отображаем ФИО в TextBox
                ClientTB.Text = selectedItem.Surname + " " + selectedItem.Name + " " + selectedItem.Patronymic;

                Grid1.Visibility = Visibility.Visible;
                SaveBTN.Visibility = Visibility.Visible;
                Grid2.Visibility = Visibility.Collapsed;
                Grid3.Visibility = Visibility.Collapsed;
                Grid4.Visibility = Visibility.Collapsed;
            }
            else
            {
                ClientTB.Text = contextRecord.Client.Surname + " " + contextRecord.Client.Name + " " + contextRecord.Client.Patronymic;
            }
        }

        private void Close2BTN_Click(object sender, RoutedEventArgs e)
        {
            Grid1.Visibility = Visibility.Visible;
            SaveBTN.Visibility = Visibility.Visible;
            Grid2.Visibility = Visibility.Collapsed;
            Grid3.Visibility = Visibility.Collapsed;
            Grid4.Visibility = Visibility.Collapsed;
        }

        private void ServicesLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ServicesLV.SelectedItem != null)
            {
                dynamic selectedItem = ServicesLV.SelectedItem;

                ServiceTB.Text = selectedItem.Name;
                PriceServiceTB.Text = selectedItem.Price.ToString();
                FinalPriceTB.Text = PriceServiceTB.Text;
                contextRecord.FinalPrice = Convert.ToDecimal(FinalPriceTB.Text.Trim());
                contextRecord.ID_Discount = 1;

                Grid1.Visibility = Visibility.Visible;
                SaveBTN.Visibility = Visibility.Visible;
                Grid2.Visibility = Visibility.Collapsed;
                Grid3.Visibility = Visibility.Collapsed;
                Grid4.Visibility = Visibility.Collapsed;
            }
            else
            {
                ServiceTB.Text = contextRecord.Service.Name;
                PriceServiceTB.Text = contextRecord.Service.Price.ToString();
            }
        }

        private void DiscountCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Discount selectedDiscount = (Discount)DiscountCB.SelectedItem;
            Client selectedClient = (Client)ClientsLV.SelectedItem;
            dynamic selectedItem = ServicesLV.SelectedItem;
            if (ServicesLV.SelectedItem != null)
            {
                if (DiscountCB.SelectedIndex == 0)
                {
                    PriceServiceTB.Text = selectedItem.Price.ToString();
                    FinalPriceTB.Text = PriceServiceTB.Text;
                    contextRecord.FinalPrice = Convert.ToDecimal(FinalPriceTB.Text.Trim());
                    contextRecord.ID_Discount = 1;
                }
                else
                {
                    if (DiscountCB.SelectedIndex == 1)
                    {
                        if (records.Any(i => i.ID_Client == selectedClient.ID))
                        {
                            MessageBox.Show($"Клиенту {selectedClient.Name} эта скидка недоступна.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
                            DiscountCB.SelectedIndex = 0;
                            return;
                        }
                        else if (records.Any(i => i.ID_Client == contextRecord.ID_Client))
                        {
                            MessageBox.Show($"Клиенту {contextRecord.Client.Name} эта скидка недоступна.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
                            DiscountCB.SelectedIndex = 0;
                            return;
                        }
                        else
                        {
                            FinalPriceTB.Text = (selectedItem.Price - selectedItem.Price / 100 * 20).ToString();
                            contextRecord.FinalPrice = Convert.ToDecimal(FinalPriceTB.Text.Trim());
                            contextRecord.ID_Discount = 2;
                        }
                    }
                    else if (DiscountCB.SelectedIndex == 2)
                    {
                        if (Convert.ToDateTime(selectedClient.DateOfBirth).Day == DateTime.Now.Day)
                        {
                            FinalPriceTB.Text = (selectedItem.Price - selectedItem.Price / 100 * 30).ToString();
                            contextRecord.FinalPrice = Convert.ToDecimal(FinalPriceTB.Text.Trim());
                            contextRecord.ID_Discount = 3;
                        }
                        else if (Convert.ToDateTime(contextRecord.Client.DateOfBirth).Day == DateTime.Now.Day)
                        {
                            FinalPriceTB.Text = (selectedItem.Price - selectedItem.Price / 100 * 30).ToString();
                            contextRecord.FinalPrice = Convert.ToDecimal(FinalPriceTB.Text.Trim());
                            contextRecord.ID_Discount = 3;
                        }
                        else
                        {
                            MessageBox.Show($"Клиенту {selectedClient.Name} эта скидка недоступна.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
                            DiscountCB.SelectedIndex = 0;
                            return;
                        }
                    }
                    else if (DiscountCB.SelectedIndex == 3)
                    {
                        if (selectedItem.TypeOfService.Name == "SPA для лица")
                        {
                            FinalPriceTB.Text = (selectedItem.Price - selectedItem.Price / 100 * 2).ToString();
                            contextRecord.FinalPrice = Convert.ToDecimal(FinalPriceTB.Text.Trim());
                            contextRecord.ID_Discount = 4;
                        }
                        else if (contextRecord.Service.TypeOfService.Name == "SPA для лица")
                        {
                            FinalPriceTB.Text = (selectedItem.Price - selectedItem.Price / 100 * 2).ToString();
                            contextRecord.FinalPrice = Convert.ToDecimal(FinalPriceTB.Text.Trim());
                            contextRecord.ID_Discount = 4;
                        }
                        else
                        {
                            MessageBox.Show("Эта скидка применяется только к программе массажа лица.", "Внимание",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                            DiscountCB.SelectedIndex = 0;
                            FinalPriceTB.Text = PriceServiceTB.Text;
                            contextRecord.FinalPrice = Convert.ToDecimal(FinalPriceTB.Text.Trim());
                            contextRecord.ID_Discount = 1;
                            return;
                        }
                    }
                }
            }
            else
            {
                ServiceTB.Text = contextRecord.Service.Name;
                PriceServiceTB.Text = contextRecord.Service.Price.ToString();
            }
        }

        private void Close3BTN_Click(object sender, RoutedEventArgs e)
        {
            Grid1.Visibility = Visibility.Visible;
            SaveBTN.Visibility = Visibility.Visible;
            Grid2.Visibility = Visibility.Collapsed;
            Grid3.Visibility = Visibility.Collapsed;
            Grid4.Visibility = Visibility.Collapsed;
        }

        private bool IsDigit(string text)
        {
            return Regex.IsMatch(text, "[0-9]");
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DateDP.SelectedDate < DateTime.Now.Date)
            {
                MessageBox.Show("Нельзя выбрать прошедшую дату.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                DateDP.SelectedDate = null;
            }
        }

        private void Search1TB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Search1TB.Text.Length > 0)

                WorkersLV.ItemsSource = DBConnection.massageSalon.Worker.Where(i => i.Position.Name == "Массажист" && i.Surname.ToLower().StartsWith(Search1TB.Text.Trim().ToLower())
                || i.Name.ToLower().StartsWith(Search1TB.Text.Trim().ToLower()) || i.Patronymic.ToLower().StartsWith(Search1TB.Text.Trim().ToLower())).ToList();

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
            {
                ServicesLV.ItemsSource = DBConnection.massageSalon.Service.Where(i => i.Name.ToLower().StartsWith(Search3TB.Text.Trim().ToLower())).ToList();
            }
            else
            {
                ServicesLV.ItemsSource = DBConnection.massageSalon.Service.ToList();
            }
        }

        private void TimeTP_SelectedTimeChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            TimeTB.Text = TimeTP.SelectedTime.ToString();
        }
    }
}

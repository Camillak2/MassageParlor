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
            genders = DBConnection.massageSalon.Gender.ToList();
            services = DBConnection.massageSalon.Service.ToList();
            types = DBConnection.massageSalon.TypeOfService.ToList();
            discounts = DBConnection.massageSalon.Discount.ToList();
            records = DBConnection.massageSalon.Record.ToList();
            DiscountCB.SelectedIndex = 0;
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
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(MassagistTB.Text) 
                    || string.IsNullOrWhiteSpace(ServiceTB.Text) 
                    || string.IsNullOrWhiteSpace(ClientTB.Text) 
                    || DiscountCB.SelectedItem == null
                    || DateDP.SelectedDate == null
                    || TimeTP.SelectedTime == null)
                {
                    MessageBox.Show("Заполните все поля.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                else
                {
                    if ((WorkersLV.SelectedItem is Worker worker) && (ClientsLV.SelectedItem is Client client) && (ServicesLV.SelectedItem is Service service))
                    {
                        record.ID_Worker = worker.ID;
                        record.ID_Client = client.ID;
                        record.ID_Service = service.ID;
                    }
                    else
                    {
                        MessageBox.Show("Заполните все поля.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    if (DateDP.SelectedDate == null || TimeTP.SelectedTime == null)
                    {
                        MessageBox.Show("Пожалуйста, выберите дату и время.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    else
                    {
                        DateTime recordDateTime = DateDP.SelectedDate.Value.Date.Add(TimeTP.SelectedTime.Value.TimeOfDay);
                        record.DateTime = recordDateTime;
                    }

                    DBConnection.massageSalon.Record.Add(record);
                    DBConnection.massageSalon.SaveChanges();
                    Close();
                }
            }
            catch
            {
                MessageBox.Show("Непредвиденная ошибка.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        };

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
                dynamic selectedItem = WorkersLV.SelectedItem;

                MassagistTB.Text = selectedItem.Surname + " " + selectedItem.Name + " " + selectedItem.Patronymic;

                Grid1.Visibility = Visibility.Visible;
                SaveBTN.Visibility = Visibility.Visible;
                Grid2.Visibility = Visibility.Collapsed;
                Grid3.Visibility = Visibility.Collapsed;
                Grid4.Visibility = Visibility.Collapsed;
            }
            else
            {
                MassagistTB.Text = "";
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
                dynamic selectedItem = ClientsLV.SelectedItem;

                ClientTB.Text = selectedItem.Surname + " " + selectedItem.Name + " " + selectedItem.Patronymic;

                Grid1.Visibility = Visibility.Visible;
                SaveBTN.Visibility = Visibility.Visible;
                Grid2.Visibility = Visibility.Collapsed;
                Grid3.Visibility = Visibility.Collapsed;
                Grid4.Visibility = Visibility.Collapsed;
            }
            else
            {
                ClientTB.Text = "";
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
                record.FinalPrice = Convert.ToDecimal(FinalPriceTB.Text.Trim());
                record.ID_Discount = 1;

                Grid1.Visibility = Visibility.Visible;
                SaveBTN.Visibility = Visibility.Visible;
                Grid2.Visibility = Visibility.Collapsed;
                Grid3.Visibility = Visibility.Collapsed;
                Grid4.Visibility = Visibility.Collapsed;
            }
            else
            {
                ServiceTB.Text = "";
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

            dynamic selectedItem = ServicesLV.SelectedItem;
            if (ServicesLV.SelectedItem != null)
            {
                if (DiscountCB.SelectedIndex == 0)
                {
                    PriceServiceTB.Text = selectedItem.Price.ToString();
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
                            MessageBox.Show($"Клиенту {selectedClient.Name} эта скидка недоступна.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
                            DiscountCB.SelectedIndex = 0;
                            return;
                        }
                        else
                        {
                            FinalPriceTB.Text = (selectedItem.Price - selectedItem.Price / 100 * 20).ToString();
                            record.FinalPrice = Convert.ToDecimal(FinalPriceTB.Text.Trim());
                            record.ID_Discount = 2;
                        }
                    }
                    else if (DiscountCB.SelectedIndex == 2)
                    {
                        if (Convert.ToDateTime(selectedClient.DateOfBirth).Day == DateTime.Now.Day)
                        {
                            FinalPriceTB.Text = (selectedItem.Price - selectedItem.Price / 100 * 30).ToString();
                            record.FinalPrice = Convert.ToDecimal(FinalPriceTB.Text.Trim());
                            record.ID_Discount = 3;
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
                            record.FinalPrice = Convert.ToDecimal(FinalPriceTB.Text.Trim());
                            record.ID_Discount = 4;
                        }
                        else
                        {
                            MessageBox.Show("Эта скидка применяется только к программе массажа лица.", "Внимание", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                            DiscountCB.SelectedIndex = 0;
                            FinalPriceTB.Text = PriceServiceTB.Text;
                            record.FinalPrice = Convert.ToDecimal(FinalPriceTB.Text.Trim());
                            record.ID_Discount = 1;
                            return;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите услугу.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CancelBTN_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

using MassageParlor.DB;
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
        public static List<TypeOfService> types { get; set; }

        public static Record record = new Record();

        public AddRecordWindow()
        {
            InitializeComponent();
            workers = DBConnection.massageSalon.Worker.ToList();
            clients = DBConnection.massageSalon.Client.ToList();
            genders = DBConnection.massageSalon.Gender.ToList();
            services = DBConnection.massageSalon.Service.ToList();
            types = DBConnection.massageSalon.TypeOfService.ToList();
            Refresh();
            this.DataContext = this;

            TimeTB.TextChanged += TimeTB_TextChanged;

            DateDP.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, DateTime.Now.Date.AddDays(-1)));
        }

        public void Refresh()
        {
            WorkersLV.ItemsSource = DBConnection.massageSalon.Worker.Where(i => i.Position.Name == "Массажист").ToList();
            ClientsLV.ItemsSource = DBConnection.massageSalon.Client.ToList();
            ServicesLV.ItemsSource = DBConnection.massageSalon.Service.ToList();
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(MassagistTB.Text) || string.IsNullOrWhiteSpace(ServiceTB.Text) || string.IsNullOrWhiteSpace(ClientTB.Text) ||
                        DateDP.SelectedDate == null || string.IsNullOrWhiteSpace(TimeTB.Text))
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
                    record.Date = DateDP.SelectedDate;
                    record.Time = TimeTB.Text.Trim();

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

        private void Choose1BTN_Click(object sender, RoutedEventArgs e)
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
                // Если строка не выбрана, очищаем TextBox
                MassagistTB.Text = "";
            }
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
                // Если строка не выбрана, очищаем TextBox
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
                // Если строка не выбрана, очищаем TextBox
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
                // Получаем выбранный объект
                dynamic selectedItem = ServicesLV.SelectedItem;

                // Отображаем ФИО в TextBox
                ServiceTB.Text = selectedItem.Name;

                Grid1.Visibility = Visibility.Visible;
                SaveBTN.Visibility = Visibility.Visible;
                Grid2.Visibility = Visibility.Collapsed;
                Grid3.Visibility = Visibility.Collapsed;
                Grid4.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Если строка не выбрана, очищаем TextBox
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

        private void TimeTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
                return;
            }

            TimeTB.Text = Regex.Replace(TimeTB.Text, @"\s", "");
            TimeTB.CaretIndex = TimeTB.Text.Length;

            if (TimeTB.Text.Length >= 5 && !string.IsNullOrEmpty(e.Text))
            {
                e.Handled = true;
                return;
            }

            // Форматирование маски ##:##
            if (TimeTB.Text.Length == 2 && !TimeTB.Text.Contains(":"))
            {
                TimeTB.Text += ":";
                TimeTB.SelectionStart = TimeTB.Text.Length;
            }

            switch (TimeTB.Text.Length)
            {
                case 0:
                    if (e.Text != "1") e.Handled = true;
                    break;
                case 1:
                    if (int.Parse(e.Text) < 0 || int.Parse(e.Text) > 9) e.Handled = true;
                    break;
                case 3:
                    if (int.Parse(e.Text) < 0 || int.Parse(e.Text) > 5) e.Handled = true;
                    break;
                case 4:
                    if (int.Parse(e.Text) < 0 || int.Parse(e.Text) > 9) e.Handled = true;
                    break;
            }

            // Проверка на 4 цифры
        }

        private void TimeTB_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TimeTB.Text.Length < 4)
            {
                MessageBox.Show("Неверный формат времени.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                // Сохранение времени в базе данных
                DBConnection.massageSalon.SaveChanges();
            }
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

        private void TimeTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            TimeTB.Text = Regex.Replace(TimeTB.Text, @"\s", "");
            TimeTB.CaretIndex = TimeTB.Text.Length;
        }
    }
}

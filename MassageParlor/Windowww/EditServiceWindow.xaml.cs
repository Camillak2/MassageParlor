using MassageParlor.DB;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Логика взаимодействия для EditServiceWindow.xaml
    /// </summary>
    public partial class EditServiceWindow : Window
    {
        public static List<Service> services { get; set; }
        public static List<TypeOfService> typeOfServices { get; set; }

        Service contextService;

        public EditServiceWindow(Service service)
        {
            InitializeComponent();
            contextService = service;

            services = DBConnection.massageSalon.Service.ToList();
            typeOfServices = DBConnection.massageSalon.TypeOfService.ToList();
            NameTB.Text = contextService.Name;
            CostTB.Text = contextService.Price.ToString();
            TypeTB.Text = contextService.TypeOfService.Name;
            this.DataContext = this;

            CostTB.TextChanged += CostTB_TextChanged;
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder error = new StringBuilder();
                Service service = contextService;
                if (string.IsNullOrWhiteSpace(NameTB.Text) || string.IsNullOrWhiteSpace(CostTB.Text))
                {
                    error.AppendLine("Заполните все поля!");
                }
                if (error.Length > 0)
                {
                    MessageBox.Show(error.ToString());
                }
                else
                {
                    if (decimal.TryParse(CostTB.Text, out decimal cost))
                    {
                        if (cost > 10000)
                        {
                            MessageBox.Show("Услуга не может быть дороже 10000 рублей!");
                            return;
                        }
                        else
                        {
                            service.Price = Convert.ToDecimal(CostTB.Text.Trim());
                        }
                    }
                    service.Name = NameTB.Text.Trim();

                    EditBTN.Visibility = Visibility.Visible;
                    SaveBTN.Visibility = Visibility.Collapsed;
                    NameTB.IsReadOnly = true;
                    CostTB.IsReadOnly = true;

                    DBConnection.massageSalon.SaveChanges();
                    Close();
                }
            }
            catch
            {
                MessageBox.Show("Заполните все поля!");
            }
        }

        private void EditBTN_Click(object sender, RoutedEventArgs e)
        {
            EditBTN.Visibility = Visibility.Collapsed;
            SaveBTN.Visibility = Visibility.Visible;
            NameTB.IsReadOnly = false;
            CostTB.IsReadOnly = false;
        }

        private void NameTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[а-яА-Я]$");
            e.Handled = !regex.IsMatch(e.Text);

            TextBox textBox = (TextBox)sender;
            string currentText = textBox.Text;

            if (currentText.Length >= 50 && !string.IsNullOrEmpty(e.Text))
            {
                e.Handled = true;
                return;
            }
        }

        private void CostTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9]$");
            e.Handled = !regex.IsMatch(e.Text);

            TextBox textBox = (TextBox)sender;
            string currentText = textBox.Text;

            if (currentText.Length >= 16 && !string.IsNullOrEmpty(e.Text))
            {
                e.Handled = true;
                return;
            }
        }

        private void CostTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            CostTB.Text = Regex.Replace(CostTB.Text, @"\s", "");
            CostTB.CaretIndex = CostTB.Text.Length;

            if (decimal.TryParse(CostTB.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal amount))
            {
                CostTB.Text = amount.ToString("F2");
                CostTB.CaretIndex = CostTB.Text.IndexOf(',');
            }
            else
            {
                // Обработка ошибки, если ввод не является числом
                MessageBox.Show("Неверный формат числа.");
                return;
            }

            if (decimal.TryParse(CostTB.Text, out decimal cost))
            {
                if (cost > 10000)
                {
                    MessageBox.Show("Услуга не может быть дороже 10000 рублей!");
                    return;
                }
            }
        }
    }
}

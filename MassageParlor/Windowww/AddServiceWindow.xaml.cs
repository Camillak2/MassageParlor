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
    /// Логика взаимодействия для AddServiceWindow.xaml
    /// </summary>
    public partial class AddServiceWindow : Window
    {
        Worker loggedWorker;

        public static List<TypeOfService> types { get; set; }
        public static List<Service> services { get; set; }
        TypeOfService contextType;

        public AddServiceWindow(TypeOfService typeOfService)
        {
            InitializeComponent();
            contextType = typeOfService;
            loggedWorker = DBConnection.loginedWorker;
            InitializeDataInPage();
            this.DataContext = this;

            CostTB.TextChanged += CostTB_TextChanged;
        }

        private void InitializeDataInPage()
        {
            types = DBConnection.massageSalon.TypeOfService.Where(i => i.Name == contextType.Name).ToList();
            TypeTB.Text = Convert.ToString(contextType.Name);
            services = DBConnection.massageSalon.Service.ToList();
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            Service service = new Service();
            try
            {
                if (string.IsNullOrWhiteSpace(NameTB.Text) || string.IsNullOrWhiteSpace(CostTB.Text))
                {
                    MessageBox.Show("Заполните все поля.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                else
                {
                    if (decimal.TryParse(CostTB.Text, out decimal cost))
                    {
                        if (cost > 10000)
                        {
                            MessageBox.Show("Услуга не может быть дороже 10000 рублей.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        else
                        {
                            service.Price = Convert.ToDecimal(CostTB.Text.Trim());
                        }
                    }
                    service.Name = NameTB.Text.Trim();
                    service.ID_TypeOfService = contextType.ID;
                    DBConnection.massageSalon.Service.Add(service);
                    DBConnection.massageSalon.SaveChanges();
                    InitializeDataInPage();
                    Close();
                }
            }
            catch
            {
                MessageBox.Show("Непредвиденная ошибка.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
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

            if (currentText.Length >= 8 && !string.IsNullOrEmpty(e.Text))
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
                MessageBox.Show("Неверный формат числа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (decimal.TryParse(CostTB.Text, out decimal cost))
            {
                if (cost > 10000)
                {
                    MessageBox.Show("Услуга не может быть дороже 10000 рублей.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
        }
    }
}

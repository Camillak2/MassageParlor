using MassageParlor.DB;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

        public static Service service = new Service();

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
            try
            {
                if (string.IsNullOrWhiteSpace(NameTB.Text) || string.IsNullOrWhiteSpace(DescriptionTB.Text) || string.IsNullOrWhiteSpace(CostTB.Text)
                    || string.IsNullOrWhiteSpace(DurationTB.Text))
                {
                    MessageBox.Show("Заполните все поля.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                else
                {
                    if (decimal.TryParse(CostTB.Text, out decimal cost))
                    {
                        if (cost > 30000)
                        {
                            MessageBox.Show("Услуга не может быть дороже 30000 рублей.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        else if (cost < 1000)
                        {
                            MessageBox.Show("Услуга не может быть дешевле 1000 рублей.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        else
                        {
                            service.Price = Convert.ToDecimal(CostTB.Text.Trim());
                        }
                    }

                    if (DurationTB.Text.Trim().Length < 4)
                    {
                        MessageBox.Show("Неверный формат времени.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else
                    {
                        service.Duration = TimeSpan.Parse(DurationTB.Text.Trim());
                    }
                    service.Name = NameTB.Text.Trim();
                    service.Description = DescriptionTB.Text.Trim();
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

        private void DurationTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
                return;
            }

            DurationTB.Text = Regex.Replace(DurationTB.Text, @"\s", "");
            DurationTB.CaretIndex = DurationTB.Text.Length;

            if (DurationTB.Text.Length >= 5 && !string.IsNullOrEmpty(e.Text))
            {
                e.Handled = true;
                return;
            }

            // Форматирование маски ##:##
            if (DurationTB.Text.Length == 2 && !DurationTB.Text.Contains(":"))
            {
                DurationTB.Text += ":";
                DurationTB.SelectionStart = DurationTB.Text.Length;
            }
        }

        private void DurationTB_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DurationTB.Text.Length < 4)
            {
                MessageBox.Show("Неверный формат времени.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }


        private void DurationTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            DurationTB.Text = Regex.Replace(DurationTB.Text, @"\s", "");
            DurationTB.CaretIndex = DurationTB.Text.Length;
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
        }

        private void AddPhotoBTN_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "*.png|*.png|*.jpeg|*.jpeg|*.jpg|*.jpg"
            };
            if (openFileDialog.ShowDialog().GetValueOrDefault())
            {
                service.Image = File.ReadAllBytes(openFileDialog.FileName);
                PhotoService.Source = new BitmapImage(new Uri(openFileDialog.FileName));

                AddPhotoTB.Text = "Изменить фото";
            }
        }

        private void CancelBTN_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

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
            DescriptionTB.Text = contextService.Description;
            DurationTB.Text = contextService.Duration.ToString();
            CostTB.Text = contextService.Price.ToString();
            TypeTB.Text = contextService.TypeOfService.Name;

            if (contextService.Image != null)
            {
                using (var stream = new MemoryStream(contextService.Image))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    PhotoService.Source = bitmap;
                }
            }

            this.DataContext = this;

            CostTB.TextChanged += CostTB_TextChanged;
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Service service = contextService;
                if (string.IsNullOrWhiteSpace(NameTB.Text) || string.IsNullOrWhiteSpace(CostTB.Text))
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
                    service.Name = NameTB.Text.Trim();
                    service.Description = DescriptionTB.Text.Trim();

                    EditBTN.Visibility = Visibility.Visible;
                    SaveBTN.Visibility = Visibility.Collapsed;
                    NameTB.IsReadOnly = true;
                    CostTB.IsReadOnly = true;
                    DescriptionTB.IsReadOnly = true;
                    DurationTB.IsReadOnly = true;
                    AddPhotoBTN.Visibility = Visibility.Hidden;

                    if (DurationTB.Text.Trim().Length < 4)
                    {
                        MessageBox.Show("Неверный формат времени.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else
                    {
                        service.Duration = TimeSpan.Parse(DurationTB.Text.Trim());
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

        private void EditBTN_Click(object sender, RoutedEventArgs e)
        {
            NameTB.IsReadOnly = false;
            DescriptionTB.IsReadOnly = false;
            DurationTB.IsReadOnly = false;
            CostTB.IsReadOnly = false;
            AddPhotoBTN.Visibility = Visibility.Visible;
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
                contextService.Image = File.ReadAllBytes(openFileDialog.FileName);
                PhotoService.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void CancelBTN_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

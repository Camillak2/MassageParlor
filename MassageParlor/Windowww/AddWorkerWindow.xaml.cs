using MassageParlor.DB;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MassageParlor.Windowww
{
    /// <summary>
    /// Логика взаимодействия для AddWorkerWindow.xaml
    /// </summary>
    public partial class AddWorkerWindow : Window
    {
        public static List<Worker> workers { get; set; }
        public static List<Position> positions { get; set; }
        public static List<Gender> genders { get; set; }

        public static Worker worker = new Worker();

        public AddWorkerWindow()
        {
            InitializeComponent();
            workers = DBConnection.massageSalon.Worker.ToList();
            positions = DBConnection.massageSalon.Position.ToList();
            genders = DBConnection.massageSalon.Gender.ToList();
            this.DataContext = this;
        }

        private void AddPhotoBTN_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "*.png|*.png|*.jpeg|*.jpeg|*.jpg|*.jpg"
            };
            if (openFileDialog.ShowDialog().GetValueOrDefault())
            {
                worker.Photo = File.ReadAllBytes(openFileDialog.FileName);
                PhotoWorker.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder error = new StringBuilder();
                if (string.IsNullOrWhiteSpace(SurnameTB.Text) || string.IsNullOrWhiteSpace(NameTB.Text) || string.IsNullOrWhiteSpace(PatronymicTB.Text) ||
                        DateOfBirthDP.SelectedDate == null || string.IsNullOrWhiteSpace(PassportTB.Text) || string.IsNullOrWhiteSpace(PhoneTB.Text) ||
                        string.IsNullOrWhiteSpace(LoginTB.Text) || PositionCB.SelectedItem == null || GenderCB.SelectedItem == null)
                {
                    error.AppendLine("Заполните все поля!");
                }
                if (DateOfBirthDP.SelectedDate != null && (DateTime.Now - (DateTime)DateOfBirthDP.SelectedDate).TotalDays < 365 * 18 + 4)
                {
                    error.AppendLine("Сотрудник не может быть младше 18 лет.");
                }
                if (error.Length > 0)
                {
                    MessageBox.Show(error.ToString());
                }
                else
                {
                    worker.Surname = SurnameTB.Text.Trim();
                    worker.Name = NameTB.Text.Trim();
                    worker.Patronymic = PatronymicTB.Text.Trim();
                    worker.Phone = PhoneTB.Text.Trim();
                    worker.DateOfBirth = DateOfBirthDP.SelectedDate;
                    worker.PassportDetails = PassportTB.Text.Trim();
                    worker.Login = LoginTB.Text.Trim();
                    worker.Password = PasswordTB.Text.Trim();

                    var a = PositionCB.SelectedItem as Position;
                    worker.ID_Position = a.ID;

                    var b = GenderCB.SelectedItem as Gender;
                    worker.ID_Gender = b.ID;

                    DBConnection.massageSalon.Worker.Add(worker);
                    DBConnection.massageSalon.SaveChanges();
                    Close();
                }
            }
            catch
            {
                MessageBox.Show("Заполните все поля!");
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Проверяем, что введенный символ - русская буква
            Regex regex = new Regex(@"^[а-яА-Я]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker datePicker = (DatePicker)sender;
            DateTime? selectedDate = datePicker.SelectedDate;

            if (selectedDate.HasValue)
            {
                DateTime today = DateTime.Today;
                int age = today.Year - selectedDate.Value.Year;

                // Учитываем, прошел ли день рождения в этом году
                if (selectedDate.Value.Date > today.AddYears(-age)) age--;

                if (age < 18)
                {
                    MessageBox.Show("Сотрудник должен быть старше 18 лет.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    datePicker.SelectedDate = null; // Сбрасываем выбранную дату
                }
            }
        }

        private void SavePhoneNumber(string phoneNumber)
        {
            try
            {
                var phoneNumberEntity = worker;
                if (phoneNumberEntity != null)
                {
                    phoneNumberEntity.Phone = phoneNumber; // Обновите номер телефона
                    DBConnection.massageSalon.SaveChanges(); // Сохраните изменения в базе данных
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении номера телефона: " + ex.Message);
            }
        }

        private string FormatPhoneNumber(string phoneNumber)
        {
            string digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());
            if (digitsOnly.Length != 10) return phoneNumber; // Вернуть исходный номер, если он некорректный

            return String.Format("+7 ({0:###}) {1:###}-{2:##}-{3:##}",
                digitsOnly.Substring(0, 3),
                digitsOnly.Substring(3, 3),
                digitsOnly.Substring(6, 2),
                digitsOnly.Substring(8, 2));
        }

        // Обработчик события PreviewTextInput для TextBox
        private void PhoneTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
                return;
            }

            TextBox textBox = (TextBox)sender;
            string currentText = textBox.Text.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");

            // Ограничиваем ввод до 11 цифр
            if (currentText.Length >= 10 && !string.IsNullOrEmpty(e.Text))
            {
                e.Handled = true;
                return;
            }

            // Применяем маску
            string maskedText = FormatPhoneNumber(currentText + e.Text);
            textBox.Text = maskedText;
            textBox.CaretIndex = maskedText.Length;
            e.Handled = true;
        }

        // Обработчик события LostFocus для TextBox
        private void PhoneTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string currentText = textBox.Text.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");

            if (currentText.Length < 10)
            {
                MessageBox.Show("Номер телефона должен содержать 11 цифр.");
                textBox.Focus();
            }
            else
            {
                // Сохранить номер телефона в базе данных
                SavePhoneNumber(currentText);
            }
        }

        private void SavePassportNumber(string passportNumber)
        {
            try
            {
                var phoneNumberEntity = worker;
                if (phoneNumberEntity != null)
                {
                    phoneNumberEntity.PassportDetails = passportNumber; // Обновите номер телефона
                    DBConnection.massageSalon.SaveChanges(); // Сохраните изменения в базе данных
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении паспортных данных: " + ex.Message);
            }
        }

        private string FormatPassportNumber(string passportNumber)
        {
            string digitsOnly = new string(passportNumber.Where(char.IsDigit).ToArray());
            if (digitsOnly.Length != 10) return passportNumber; // Вернуть исходный номер, если он некорректный

            return String.Format("{0:####} {1:######}",
                digitsOnly.Substring(0, 4),
                digitsOnly.Substring(3, 6));
        }

        private void PassportTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
                return;
            }

            TextBox textBox = (TextBox)sender;
            string currentText = textBox.Text.Replace(" ", "").Replace(" ", "");

            // Ограничиваем ввод до 11 цифр
            if (currentText.Length >= 9 && !string.IsNullOrEmpty(e.Text))
            {
                e.Handled = true;
                return;
            }

            // Применяем маску
            string maskedText = FormatPassportNumber(currentText + e.Text);
            textBox.Text = maskedText;
            textBox.CaretIndex = maskedText.Length;
            e.Handled = true;
        }

        private void PassportTB_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string currentText = textBox.Text.Replace(" ", "").Replace(" ", "");

            if (currentText.Length < 9)
            {
                MessageBox.Show("Паспортные данные должны содержать 10 цифр.");
            }
            else
            {
                // Сохранить номер телефона в базе данных
                SavePassportNumber(currentText);
            }
        }
    }
}

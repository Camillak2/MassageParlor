using MassageParlor.DB;
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
    /// Логика взаимодействия для AddClientWindow.xaml
    /// </summary>
    public partial class AddClientWindow : Window
    {
        public static Client client = new Client();

        public static List<Gender> genders {  get; set; }
        public static List<Client> clients { get; set; }

        public AddClientWindow()
        {
            InitializeComponent();
            genders = DBConnection.massageSalon.Gender.ToList();
            clients = DBConnection.massageSalon.Client.ToList();
            this.DataContext = this;
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder error = new StringBuilder();
                if (string.IsNullOrWhiteSpace(SurnameTB.Text) || string.IsNullOrWhiteSpace(NameTB.Text) || string.IsNullOrWhiteSpace(PatronymicTB.Text) ||
                        DateOfBirthDP.SelectedDate == null || string.IsNullOrWhiteSpace(PhoneTB.Text) || GenderCB.SelectedItem == null)
                {
                    error.AppendLine("Заполните все поля!");
                }
                if (error.Length > 0)
                {
                    MessageBox.Show(error.ToString());
                }
                else
                {
                    client.Surname = SurnameTB.Text.Trim();
                    client.Name = NameTB.Text.Trim();
                    client.Patronymic = PatronymicTB.Text.Trim();
                    client.Phone = PhoneTB.Text.Trim();
                    client.DateOfBirth = DateOfBirthDP.SelectedDate;

                    var b = GenderCB.SelectedItem as Gender;
                    client.ID_Gender = b.ID;

                    DBConnection.massageSalon.Client.Add(client);
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

                if (age < 7)
                {
                    MessageBox.Show("Клиент не может быть младше 7 лет.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    datePicker.SelectedDate = null; // Сбрасываем выбранную дату
                }
            }
        }

        private void SavePhoneNumber(string phoneNumber)
        {
            try
            {
                var phoneNumberEntity = client;
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
    }
}

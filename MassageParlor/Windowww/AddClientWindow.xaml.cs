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

            SurnameTB.TextChanged += TextBox_TextChanged;
            NameTB.TextChanged += TextBox_TextChanged;
            PatronymicTB.TextChanged += TextBox_TextChanged;
            PhoneTB.TextChanged += TextBox_TextChanged;
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SurnameTB.Text) || string.IsNullOrWhiteSpace(NameTB.Text) || string.IsNullOrWhiteSpace(PatronymicTB.Text) ||
                   DateOfBirthDP.SelectedDate == null || string.IsNullOrWhiteSpace(PhoneTB.Text) || GenderCB.SelectedItem == null)
                {
                    MessageBox.Show("Заполните все поля.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                else
                {
                    client.Surname = SurnameTB.Text.Trim();
                    client.Name = NameTB.Text.Trim();
                    client.Patronymic = PatronymicTB.Text.Trim();
                    client.DateOfBirth = DateOfBirthDP.SelectedDate;

                    if (PhoneTB.Text.Length < 16)
                    {
                        MessageBox.Show("Номер телефона должен содержать 11 цифр.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    else
                    {
                        client.Phone = PhoneTB.Text.Trim();
                    }

                    var a = GenderCB.SelectedItem as Gender;
                    client.ID_Gender = a.ID;

                    DBConnection.massageSalon.Client.Add(client);
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

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Проверяем, что введенный символ - русская буква
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
                    MessageBox.Show("Клиент не может быть младше 7 лет.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                    datePicker.SelectedDate = null; // Сбрасываем выбранную дату
                }
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
                MessageBox.Show("Номер телефона должен содержать 11 цифр.", "Внимание", MessageBoxButton.OK,MessageBoxImage.Information);
                return;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Фамилия
            SurnameTB.Text = Regex.Replace(SurnameTB.Text, @"\s", "");
            SurnameTB.CaretIndex = SurnameTB.Text.Length;

            //Имя
            NameTB.Text = Regex.Replace(NameTB.Text, @"\s", "");
            NameTB.CaretIndex = NameTB.Text.Length;

            //Отчество
            PatronymicTB.Text = Regex.Replace(PatronymicTB.Text, @"\s", "");
            PatronymicTB.CaretIndex = PatronymicTB.Text.Length;

            //Номер телефона
            PhoneTB.Text = Regex.Replace(PhoneTB.Text, @"\s", "");
            PhoneTB.CaretIndex = PhoneTB.Text.Length;
        }
    }
}

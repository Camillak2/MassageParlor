using MassageParlor.DB;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

            SurnameTB.TextChanged += TextBox_TextChanged;
            NameTB.TextChanged += TextBox_TextChanged;
            PatronymicTB.TextChanged += TextBox_TextChanged;
            PhoneTB.TextChanged += TextBox_TextChanged;
            LoginTB.TextChanged += TextBox_TextChanged;
            PasswordTB.TextChanged += TextBox_TextChanged;
            PassportTB.TextChanged += TextBox_TextChanged;
        }

        private void AddPhotoBTN_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "*.png|*.png|*.jpeg|*.jpeg|*.jpg|*.jpg"
            };
            if (openFileDialog.ShowDialog().GetValueOrDefault())
            {
                worker.Photo = File.ReadAllBytes(openFileDialog.FileName);
                PhotoWorker.Source = new BitmapImage(new Uri(openFileDialog.FileName));

                AddPhotoTB.Text = "Изменить фото";
            }
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SurnameTB.Text) || string.IsNullOrWhiteSpace(NameTB.Text) ||
                        DateOfBirthDP.SelectedDate == null || string.IsNullOrWhiteSpace(PassportTB.Text) || string.IsNullOrWhiteSpace(PhoneTB.Text) ||
                        string.IsNullOrWhiteSpace(LoginTB.Text) || PositionCB.SelectedItem == null || GenderCB.SelectedItem == null)
                {
                    MessageBox.Show("Заполните все поля.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                else
                {
                    worker.Surname = SurnameTB.Text.Trim();
                    worker.Name = NameTB.Text.Trim();
                    worker.Patronymic = PatronymicTB.Text.Trim();
                    worker.DateOfBirth = DateOfBirthDP.SelectedDate;

                    string login = LoginTB.Text;
                    bool loginExists = DBConnection.massageSalon.Worker.Any(w => w.Login == login);

                    if (loginExists)
                    {
                        MessageBox.Show("Этот логин уже занят. Выберите другой.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    else
                    {
                        if (LoginTB.Text.Length > 13)
                        {
                            MessageBox.Show("Слишком длинный логин.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        else if (LoginTB.Text.Length < 6)
                        {
                            MessageBox.Show("Слишком короткий логин.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        else
                        {
                            worker.Login = LoginTB.Text.Trim();
                        }
                    }

                    if (PasswordTB.Text.Length > 13)
                    {
                        MessageBox.Show("Слишком длинный пароль.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    else if (PasswordTB.Text.Length < 6)
                    {
                        MessageBox.Show("Слишком короткий пароль.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);    
                        return;
                    }
                    else
                    {
                        worker.Password = PasswordTB.Text.Trim();
                    }

                    if (PhoneTB.Text.Length < 16)
                    {
                        if (PhoneTB.Text.Length < 10)
                        {
                            MessageBox.Show("Номер телефона должен содержать 11 цифр.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        else
                        {
                            worker.Phone = PhoneTB.Text.Trim();
                        }
                    }

                    if (PassportTB.Text.Length < 10)
                    {
                        MessageBox.Show("Паспортные данные должны содержать 10 цифр.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    else
                    {
                        worker.PassportDetails = PassportTB.Text;
                    }

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
                MessageBox.Show("Непредвиденная ошибка.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Проверяем, что введенный символ - русская буква
            Regex regex = new Regex(@"^[а-яА-Я]$");
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
                    MessageBox.Show("Сотрудник должен быть старше 18 лет.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
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
                MessageBox.Show("Номер телефона должен содержать 11 цифр.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        private void PassportTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Используем регулярное выражение для проверки, является ли введенный символ цифрой
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);

            if (PassportTB.Text.Length >= 10 && !string.IsNullOrEmpty(e.Text))
            {
                e.Handled = true;
                return;
            }
        }

        private void PassportTB_LostFocus(object sender, RoutedEventArgs e)
        {
            if (PassportTB.Text.Length < 10)
            {
                MessageBox.Show("Паспортные данные должны содержать 10 цифр.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
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

            //Паспорт
            PassportTB.Text = Regex.Replace(PassportTB.Text, @"\s", "");
            PassportTB.CaretIndex = PassportTB.Text.Length;

            //Логин
            LoginTB.Text = Regex.Replace(LoginTB.Text, @"\s", "");
            LoginTB.CaretIndex = LoginTB.Text.Length;

            //Пароль
            PasswordTB.Text = Regex.Replace(PasswordTB.Text, @"\s", "");
            PasswordTB.CaretIndex = PasswordTB.Text.Length;
        }
    }
}

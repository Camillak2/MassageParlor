using MassageParlor.DB;
using MassageParlor.Pages;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для EditWorkerWindow.xaml
    /// </summary>
    public partial class EditWorkerWindow : Window
    {
        public static List<Worker> workers { get; set; }
        public static List<Position> positions { get; set; }
        public static List<Gender> genders { get; set; }

        Worker contextWorker;

        public EditWorkerWindow(Worker worker)
        {
            InitializeComponent();
            contextWorker = worker;
            InitializeDataInPage();
            this.DataContext = this;

        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Проверяем, что введенный символ - русская буква
            Regex regex = new Regex(@"^[а-яА-Я]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void InitializeDataInPage()
        {
            workers = DBConnection.massageSalon.Worker.ToList();
            positions = DBConnection.massageSalon.Position.ToList();
            genders = DBConnection.massageSalon.Gender.ToList();
            this.DataContext = this;
            SurnameTB.Text = contextWorker.Surname;
            NameTB.Text = contextWorker.Name;
            PatronymicTB.Text = contextWorker.Patronymic;
            DateOfBirthDP.SelectedDate = contextWorker.DateOfBirth;
            PhoneTB.Text = contextWorker.Phone;
            PositionCB.SelectedIndex = (int)contextWorker.ID_Position - 1;
            GenderCB.SelectedIndex = (int)contextWorker.ID_Gender - 1;
            LoginTB.Text = contextWorker.Login;
            PasswordTB.Text = contextWorker.Password;
            PassportTB.Text = contextWorker.PassportDetails;
            if (contextWorker.Photo != null)
            {
                using (var stream = new MemoryStream(contextWorker.Photo))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    PhotoWorker.Source = bitmap;
                }
            }
        }

        private void EditBTN_Click(object sender, RoutedEventArgs e)
        {
            SurnameTB.IsReadOnly = false;
            NameTB.IsReadOnly = false;
            PatronymicTB.IsReadOnly = false;
            DateOfBirthDP.IsEnabled = true;
            PhoneTB.IsReadOnly = false;
            PasswordTB.IsReadOnly = false;
            PositionCB.IsEnabled = true;
            GenderCB.IsEnabled = true;
            PassportTB.IsReadOnly = false;

            SaveBTN.Visibility = Visibility.Visible;
            EditPhotoBTN.Visibility = Visibility.Visible;
            EditBTN.Visibility = Visibility.Collapsed;

            //
            SurnameTB.Text = contextWorker.Surname;
            NameTB.Text = contextWorker.Name;
            PatronymicTB.Text = contextWorker.Patronymic;
            DateOfBirthDP.SelectedDate = contextWorker.DateOfBirth;
            PositionCB.SelectedIndex = (int)contextWorker.ID_Position - 1;
            GenderCB.SelectedIndex = (int)contextWorker.ID_Gender - 1;
            LoginTB.Text = contextWorker.Login;
            PasswordTB.Text = contextWorker.Password;
            PhoneTB.Text = contextWorker.Phone;
            PassportTB.Text = contextWorker.PassportDetails;
        }

        private void EditPhotoBTN_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "*.png|*.png|*.jpeg|*.jpeg|*.jpg|*.jpg"
            };

            if (openFileDialog.ShowDialog().GetValueOrDefault())
            {
                DBConnection.loginedWorker.Photo = File.ReadAllBytes(openFileDialog.FileName);
                PhotoWorker.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder error = new StringBuilder();
                Worker worker = contextWorker;
                if (string.IsNullOrWhiteSpace(SurnameTB.Text) || string.IsNullOrWhiteSpace(NameTB.Text) || string.IsNullOrWhiteSpace(PatronymicTB.Text) ||
                DateOfBirthDP.SelectedDate == null || string.IsNullOrWhiteSpace(PhoneTB.Text) || string.IsNullOrWhiteSpace(LoginTB.Text) ||
                string.IsNullOrWhiteSpace(PasswordTB.Text))
                {
                    error.AppendLine("Заполните все поля!");
                }
                if (DateOfBirthDP.SelectedDate != null && (DateTime.Now - (DateTime)DateOfBirthDP.SelectedDate).TotalDays < 365 * 18 + 4)
                {
                    error.AppendLine("Вы должны быть старше 18 лет.");
                }
                if (error.Length > 0)
                {
                    MessageBox.Show(error.ToString());
                }
                else
                {
                    worker.Surname = SurnameTB.Text;
                    worker.Name = NameTB.Text;
                    worker.Patronymic = PatronymicTB.Text;
                    worker.DateOfBirth = DateOfBirthDP.SelectedDate;
                    worker.Phone = PhoneTB.Text;
                    worker.Login = LoginTB.Text;
                    worker.Password = PasswordTB.Text;
                    worker.PassportDetails = PassportTB.Text;
                    worker.ID_Position = (PositionCB.SelectedItem as Position).ID;
                    worker.ID_Gender = (GenderCB.SelectedItem as Gender).ID;
                    DBConnection.massageSalon.SaveChanges();


                }
                SurnameTB.IsReadOnly = true;
                NameTB.IsReadOnly = true;
                PatronymicTB.IsReadOnly = true;
                DateOfBirthDP.IsEnabled = false;
                PhoneTB.IsReadOnly = true;
                PasswordTB.IsReadOnly = true;
                PositionCB.IsEnabled = false;
                GenderCB.IsEnabled = false;
                PassportTB.IsReadOnly = true;

                SaveBTN.Visibility = Visibility.Collapsed;
                EditPhotoBTN.Visibility = Visibility.Collapsed;
                EditBTN.Visibility = Visibility.Visible;

                Close();
            }
            catch
            {
                MessageBox.Show("Произошла ошибка!");
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

                if (age < 18)
                {
                    MessageBox.Show("Вы должны быть старше 18 лет.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    datePicker.SelectedDate = null; // Сбрасываем выбранную дату
                }
            }
        }

        private void SavePhoneNumber(string phoneNumber)
        {
            try
            {
                var phoneNumberEntity = DBConnection.loginedWorker;
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
                var phoneNumberEntity = contextWorker;
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

        private void PassportTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
                return;
            }

            TextBox textBox = (TextBox)sender;
            string currentText = textBox.Text;

            // Ограничиваем ввод до 11 цифр
            if (currentText.Length >= 10 && !string.IsNullOrEmpty(e.Text))
            {
                e.Handled = true;
                return;
            }
        }

        private void PassportTB_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string currentText = textBox.Text;

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

using MassageParlor.DB;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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

namespace MassageParlor.Pages
{
    /// <summary>
    /// Логика взаимодействия для MyPersonalAccountPage.xaml
    /// </summary>
    public partial class MyPersonalAccountPage : Page
    {
        Worker loggedWorker;

        public static List<Worker> workers { get; set; }

        public MyPersonalAccountPage()
        {
            InitializeComponent();
            loggedWorker = DBConnection.loginedWorker;
            SurnameTB.Text = loggedWorker.Surname;
            NameTB.Text = loggedWorker.Name;
            PatronymicTB.Text = loggedWorker.Patronymic;
            DateOfBirthDP.SelectedDate = loggedWorker.DateOfBirth;
            PositionTB.Text = loggedWorker.Position.Name;
            GenderTB.Text = loggedWorker.Gender.Name;
            LoginTB.Text = loggedWorker.Login;
            PasswordTB.Text = loggedWorker.Password;
            PhoneTB.Text = loggedWorker.Phone;
            if (loggedWorker.Photo != null)
            {
                using (var stream = new MemoryStream(loggedWorker.Photo))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    PhotoWorker.Source = bitmap;
                }
            }
            CheckConditionAndToggleButtonVisibility();

            SurnameTB.TextChanged += TextBox_TextChanged;
            NameTB.TextChanged += TextBox_TextChanged;
            PatronymicTB.TextChanged += TextBox_TextChanged;
            PhoneTB.TextChanged += TextBox_TextChanged;
            PasswordTB.TextChanged += TextBox_TextChanged;
        }

        private void ProfileBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MyPersonalAccountPage());
        }

        private void WorkersBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new WorkersPage());
        }

        private void ClientsBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ClientsPage());
        }

        private void RecordsBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RecordsPage());
        }

        private void ServicesBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ServicesPage());
        }

        private void MassageBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MassagePage());
        }

        private void LogOutBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthorizationPage());
        }

        private void CheckConditionAndToggleButtonVisibility()
        {
            if (loggedWorker.Position.Name == "Администратор")
            {
                WorkersBTN.Visibility = Visibility.Visible;
                MassageBTN.Visibility = Visibility.Collapsed;
            }
            else if (loggedWorker.Position.Name == "Массажист")
            {
                WorkersBTN.Visibility = Visibility.Collapsed;
                MassageBTN.Visibility = Visibility.Visible;
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

            SaveBTN.Visibility = Visibility.Visible;
            EditPhotoBTN.Visibility = Visibility.Visible;
            EditBTN.Visibility = Visibility.Collapsed;

            //
            SurnameTB.Text = loggedWorker.Surname;
            NameTB.Text = loggedWorker.Name;
            PatronymicTB.Text = loggedWorker.Patronymic;
            DateOfBirthDP.SelectedDate = loggedWorker.DateOfBirth;
            PositionTB.Text = loggedWorker.Position.Name;
            GenderTB.Text = loggedWorker.Gender.Name;
            LoginTB.Text = loggedWorker.Login;
            PasswordTB.Text = loggedWorker.Password;
            PhoneTB.Text = loggedWorker.Phone;
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
                Worker worker = loggedWorker;
                if (string.IsNullOrWhiteSpace(SurnameTB.Text) || string.IsNullOrWhiteSpace(NameTB.Text) ||
                    string.IsNullOrWhiteSpace(PatronymicTB.Text) ||
                        DateOfBirthDP.SelectedDate == null || string.IsNullOrWhiteSpace(PhoneTB.Text) ||
                        string.IsNullOrWhiteSpace(LoginTB.Text) || string.IsNullOrWhiteSpace(PasswordTB.Text))
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
                    if (PasswordTB.Text.Length > 13)
                    {
                        MessageBox.Show("Слишком длинный пароль!");
                        return;
                    }
                    else if (PasswordTB.Text.Length < 6)
                    {
                        MessageBox.Show("Слишком короткий пароль!");
                        return;
                    }
                    else
                    {
                        worker.Password = PasswordTB.Text.Trim();
                    }

                    worker.Surname = SurnameTB.Text;
                    worker.Name = NameTB.Text;
                    worker.Patronymic = PatronymicTB.Text;
                    worker.DateOfBirth = DateOfBirthDP.SelectedDate;
                    worker.Phone = PhoneTB.Text;
                    worker.Login = LoginTB.Text;
                    DBConnection.massageSalon.SaveChanges();
                }

                SurnameTB.IsReadOnly = true;
                NameTB.IsReadOnly = true;
                PatronymicTB.IsReadOnly = true;
                DateOfBirthDP.IsEnabled = false;
                PhoneTB.IsReadOnly = true;
                PasswordTB.IsReadOnly = true;

                SaveBTN.Visibility = Visibility.Collapsed;
                EditPhotoBTN.Visibility = Visibility.Collapsed;
                EditBTN.Visibility = Visibility.Visible;
            }
            catch
            {
                MessageBox.Show("Произошла ошибка!");
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Проверяем, что введенный символ - русская буква
            Regex regex = new Regex(@"^[а-яА-Я]+$");
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
                return;
            }
            else
            {
                // Сохранить номер телефона в базе данных
                SavePhoneNumber(currentText);
            }
        }

        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainMenuPage());
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

            //Пароль
            PasswordTB.Text = Regex.Replace(PasswordTB.Text, @"\s", "");
            PasswordTB.CaretIndex = PasswordTB.Text.Length;
        }
    }
}

﻿using MassageParlor.DB;
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

            SurnameTB.TextChanged += TextBox_TextChanged;
            NameTB.TextChanged += TextBox_TextChanged;
            PatronymicTB.TextChanged += TextBox_TextChanged;
            PhoneTB.TextChanged += TextBox_TextChanged;
            PasswordTB.TextChanged += TextBox_TextChanged;
            PassportTB.TextChanged += TextBox_TextChanged;
        }

        //private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        //{
        //    // Проверяем, что введенный символ - русская буква
        //    Regex regex = new Regex(@"^[а-яА-Я]$");
        //    e.Handled = !regex.IsMatch(e.Text);

        //    TextBox textBox = (TextBox)sender;
        //    string currentText = textBox.Text;

        //    if (currentText.Length >= 50 && !string.IsNullOrEmpty(e.Text))
        //    {
        //        e.Handled = true;
        //        return;
        //    }
        //}

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
                contextWorker.Photo = File.ReadAllBytes(openFileDialog.FileName);
                PhotoWorker.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SurnameTB.Text) || string.IsNullOrWhiteSpace(NameTB.Text) ||
                DateOfBirthDP.SelectedDate == null || string.IsNullOrWhiteSpace(PhoneTB.Text) || string.IsNullOrWhiteSpace(LoginTB.Text) ||
                string.IsNullOrWhiteSpace(PasswordTB.Text) || GenderCB.SelectedItem == null || PositionCB.SelectedItem == null) 
                {
                    MessageBox.Show("Заполните все поля.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                else
                {
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
                        contextWorker.Password = PasswordTB.Text.Trim();
                    }

                    if (PhoneTB.Text.Length < 16)
                    {
                        MessageBox.Show("Номер телефона должен содержать 11 цифр.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    else
                    {
                        contextWorker.Phone = PhoneTB.Text.Trim();
                    }

                    if (PassportTB.Text.Length < 10)
                    {
                        MessageBox.Show("Паспортные данные должны содержать 10 цифр.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    else
                    {
                        contextWorker.PassportDetails = PassportTB.Text;
                    }

                    contextWorker.Surname = SurnameTB.Text;
                    contextWorker.Name = NameTB.Text;
                    contextWorker.Patronymic = PatronymicTB.Text;
                    contextWorker.DateOfBirth = DateOfBirthDP.SelectedDate;
                    contextWorker.ID_Position = (PositionCB.SelectedItem as Position).ID;
                    contextWorker.ID_Gender = (GenderCB.SelectedItem as Gender).ID;
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
                MessageBox.Show("Непредвиденная ошибка.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show("Сотрудник должен быть старше 18 лет.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
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
            catch
            {
                MessageBox.Show("Ошибка при сохранении номера телефона.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
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
            else
            {
                // Сохранить номер телефона в базе данных
                SavePhoneNumber(currentText);
            }
        }

        private void PassportTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
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
            Worker worker = contextWorker;
            if (PassportTB.Text.Length < 9)
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

            //Пароль
            PasswordTB.Text = Regex.Replace(PasswordTB.Text, @"\s", "");
            PasswordTB.CaretIndex = PasswordTB.Text.Length;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^[\p{IsCyrillic}]+$"))
            {
                e.Handled = true;
            }

            TextBox textBox = (TextBox)sender;
            string currentText = textBox.Text;

            if (currentText.Length >= 50 && !string.IsNullOrEmpty(e.Text))
            {
                e.Handled = true;
                return;
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = true;
            }
        }

        private void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string text = (string)e.DataObject.GetData(DataFormats.Text);
                if (!Regex.IsMatch(text, @"^[\p{IsCyrillic}]+$"))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        //для телефона
        private void PhoneTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = true;
            }
        }

        private void PhoneTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string text = (string)e.DataObject.GetData(DataFormats.Text);
                if (!Regex.IsMatch(text, @"^[0-9]+$"))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void CancelBTN_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

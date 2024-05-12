using MassageParlor.DB;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        private void LogOutBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthorizationPage());
        }

        private void EditBTN_Click(object sender, RoutedEventArgs e)
        {
            SurnameTB.IsReadOnly = false;
            NameTB.IsReadOnly = false;
            PatronymicTB.IsReadOnly = false;
            DateOfBirthDP.IsEnabled = false;
            PhoneTB.IsReadOnly = false;
            PasswordTB.IsReadOnly = false;

            SaveBTN.Visibility = Visibility.Visible;
            EditPhotoBTN.Visibility = Visibility.Visible;
            EditBTN.Visibility = Visibility.Collapsed;
        }

        private void EditPhotoBTN_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "*.png|*.png|*.jpeg|*.jpeg|*.jpg|*.jpg"
            };

            if (openFileDialog.ShowDialog().GetValueOrDefault())
            {
                DBConnection.loginedWorker.Image = File.ReadAllBytes(openFileDialog.FileName);
                PhotoWorker.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            SurnameTB.IsReadOnly = true;
            NameTB.IsReadOnly = true;
            PatronymicTB.IsReadOnly = true;
            DateOfBirthDP.IsEnabled = true;
            PhoneTB.IsReadOnly = true;
            PasswordTB.IsReadOnly = true;

            SaveBTN.Visibility = Visibility.Collapsed;
            EditPhotoBTN.Visibility = Visibility.Collapsed;
            EditBTN.Visibility = Visibility.Visible;
        }
    }
}

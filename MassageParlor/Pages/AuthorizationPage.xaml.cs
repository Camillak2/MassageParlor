using System;
using System.Collections.Generic;
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
using MassageParlor.DB;

namespace MassageParlor.Pages
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationPage.xaml
    /// </summary>
    public partial class AuthorizationPage : Page
    {
        public static List<Worker> workers { get; set; }

        public AuthorizationPage()
        {
            InitializeComponent();
        }

        private void EnterBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string login = LoginTB.Text.Trim();
                string password = PasswordTB.Password.Trim();

                workers = DBConnection.massageSalon.Worker.ToList();
                var currentWorker = workers.FirstOrDefault(i => i.Login.Trim() == login && i.Password.Trim() == password);
                DBConnection.loginedWorker = currentWorker;

                if (currentWorker != null && currentWorker.Position.Name == "Администратор")
                {
                    NavigationService.Navigate(new AdminMainMenuPage());
                }
                else if (currentWorker != null && currentWorker.Position.Name == "Массажист")
                {
                    NavigationService.Navigate(new MasseurMainMenuPage());
                }
                else
                {
                    MessageBox.Show("Проверьте правильность логина и пароля.");
                }
            }
            catch
            {
                MessageBox.Show("Возникла ошибка.");
            }
        }
    }
}

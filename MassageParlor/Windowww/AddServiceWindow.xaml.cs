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
    /// Логика взаимодействия для AddServiceWindow.xaml
    /// </summary>
    public partial class AddServiceWindow : Window
    {
        Worker loggedWorker;

        public static List<TypeOfService> types { get; set; }
        public static List<Service> services { get; set; }
        TypeOfService contextType;

        public AddServiceWindow(TypeOfService typeOfService)
        {
            InitializeComponent();
            contextType = typeOfService;
            loggedWorker = DBConnection.loginedWorker;
            InitializeDataInPage();
            this.DataContext = this;
        }

        private void InitializeDataInPage()
        {
            types = DBConnection.massageSalon.TypeOfService.Where(i => i.Name == contextType.Name).ToList();
            TypeTB.Text = Convert.ToString(contextType.Name);
            services = DBConnection.massageSalon.Service.ToList();
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            Service service = new Service();
            try
            {
                StringBuilder error = new StringBuilder();
                if (string.IsNullOrWhiteSpace(NameTB.Text) || string.IsNullOrWhiteSpace(CostTB.Text))
                {
                    error.AppendLine("Заполните все поля!");
                }
                if (error.Length > 0)
                {
                    MessageBox.Show(error.ToString());
                }
                else
                {
                    service.Name = NameTB.Text.Trim();
                    service.Price = Convert.ToInt32(CostTB.Text.Trim());
                    //service.TypeOfService.Name = TypeTB.Text.Trim();
                    service.ID_TypeOfService = contextType.ID;

                    DBConnection.massageSalon.Service.Add(service);
                    DBConnection.massageSalon.SaveChanges();
                    InitializeDataInPage();
                    Close();
                }
            }
            catch
            {
                MessageBox.Show("Заполните все поля!");
            }
        }

        private void NameTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[а-яА-Я]$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void CostTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9]$");
            e.Handled = !regex.IsMatch(e.Text);
        }
    }
}

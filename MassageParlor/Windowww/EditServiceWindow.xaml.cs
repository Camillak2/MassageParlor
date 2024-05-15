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
    /// Логика взаимодействия для EditServiceWindow.xaml
    /// </summary>
    public partial class EditServiceWindow : Window
    {
        public static List<Service> services { get; set; }
        public static List<TypeOfService> typeOfServices { get; set; }

        Service contextService;

        public EditServiceWindow(Service service)
        {
            InitializeComponent();
            contextService = service;

            services = DBConnection.massageSalon.Service.ToList();
            typeOfServices = DBConnection.massageSalon.TypeOfService.ToList();
            this.DataContext = this;
            NameTB.Text = contextService.Name;
            CostTB.Text = contextService.Price.ToString();
            TypeTB.Text = contextService.TypeOfService.Name;
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder error = new StringBuilder();
                Service service = contextService;
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
                    //service.Price = CostTB.Text.Trim();

                    EditBTN.Visibility = Visibility.Collapsed;
                    SaveBTN.Visibility = Visibility.Visible;
                    NameTB.IsReadOnly = true;
                    CostTB.IsReadOnly = true;

                    DBConnection.massageSalon.SaveChanges();
                    Close();
                }
            }
            catch
            {
                MessageBox.Show("Заполните все поля!");
            }
        }

        private void EditBTN_Click(object sender, RoutedEventArgs e)
        {
            EditBTN.Visibility = Visibility.Visible;
            SaveBTN.Visibility = Visibility.Collapsed;
            NameTB.IsReadOnly = false;
            CostTB.IsReadOnly = false;
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

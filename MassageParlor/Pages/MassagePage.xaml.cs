using MassageParlor.Windowww;
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
using System.Net.Http;

namespace MassageParlor.Pages
{
    /// <summary>
    /// Логика взаимодействия для MassagePage.xaml
    /// </summary>
    public partial class MassagePage : Page
    {
        public MassagePage()
        {
            InitializeComponent();
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button button)
            {
                DescriptionTextBlock.Text = button.Tag.ToString();
                DescriptionTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            DescriptionTextBlock.Visibility = Visibility.Collapsed;
        }

        private void ProfileBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MyPersonalAccountPage());
        }

        private void ServicesBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ServicesPage());
        }

        private void RecordsBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RecordsPage());
        }

        private void MassageBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MassagePage());
        }

        private void LogOutBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthorizationPage());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog().GetValueOrDefault())
            {
                printDialog.PrintVisual(this, "Отчёт");
            }
            //printDialog.PrintQueue.Dispose();
        }



        //private void One_Click(object sender, RoutedEventArgs e)
        //{
        //    Button button = (Button)sender;
        //    One.Background = Brushes.Red;

        //    Two.Background = Brushes.Gray;

        //    Three.Background = Brushes.Gray;

        //    hdh.Text = 1.ToString();
        //}

        //private void Two_Click(object sender, RoutedEventArgs e)
        //{
        //    Button button = (Button)sender;
        //    One.Background = Brushes.Red;

        //    Two.Background = Brushes.Red;

        //    Three.Background = Brushes.Gray;

        //    hdh.Text = 2.ToString();
        //}

        //private void Three_Click(object sender, RoutedEventArgs e)
        //{
        //    Button button = (Button)sender;
        //    One.Background = Brushes.Red;

        //    Two.Background = Brushes.Red;

        //    Three.Background = Brushes.Red;

        //    hdh.Text = 3.ToString();
        //}
    }
}

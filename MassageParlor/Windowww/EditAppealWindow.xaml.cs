﻿using MassageParlor.DB;
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
using System.Windows.Shapes;

namespace MassageParlor.Windowww
{
    /// <summary>
    /// Логика взаимодействия для EditAppealWindow.xaml
    /// </summary>
    public partial class EditAppealWindow : Window
    {
        Appeals contextAppeal;
        public static List<Taskk> tasks { get; set; }
        public static List<Worker> workers { get; set; }

        public EditAppealWindow(Appeals appeal)
        {
            InitializeComponent();
            contextAppeal = appeal;
            InitializeDataInPage();
            this.DataContext = this;
        }

        private void InitializeDataInPage()
        {
            tasks = DBConnection.massageSalon.Taskk.ToList();
            workers = DBConnection.massageSalon.Worker.ToList();
            this.DataContext = this;
            TaskCB.SelectedIndex = (int)contextAppeal.ID_Task - 1;
            DateTimeDP.SelectedDate = contextAppeal.DateTime;
            StatusTB.Text = contextAppeal.Status.Name;
        }

        private void EditBTN_Click(object sender, RoutedEventArgs e)
        {
            TaskCB.IsEnabled = true;
            SaveBTN.Visibility = Visibility.Visible;
            EditBTN.Visibility = Visibility.Collapsed;
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Appeals appeal = contextAppeal;
                if (TaskCB.SelectedItem == null)
                {
                    MessageBox.Show("Заполните все поля.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                else
                {
                    appeal.ID_Task = (TaskCB.SelectedItem as Taskk).ID;
                    DBConnection.massageSalon.SaveChanges();
                }
                SaveBTN.Visibility = Visibility.Collapsed;
                EditBTN.Visibility = Visibility.Visible;
                Close();
            }
            catch
            {
                MessageBox.Show("Заполните все поля.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        private void CancelBTN_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

using LiveCharts.Wpf;
using LiveCharts;
using MassageParlor.DB;
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
    /// Логика взаимодействия для ClientsPopularityWindow.xaml
    /// </summary>
    public partial class ClientsPopularityWindow : Window
    {
        public static List<Record> records { get; set; }
        public static List<Client> clients { get; set; }
        public static MainViewModel main { get; set; }

        public ClientsPopularityWindow()
        {
            InitializeComponent();
            records = DBConnection.massageSalon.Record.ToList();
            main = new MainViewModel();
            main.Values = new SeriesCollection();
            main.Labels = new List<string>();
            main.LoadData(DateTime.Now); // Загружаем данные для текущего месяца
            DataContext = main;

            AxesCollection axes = new AxesCollection();
            axes.Add(new Axis() { Labels = main.Labels });
            recordsChart.AxisX = axes;

            DateDP.SelectedDateChanged += DatePicker_SelectedDateChanged;
        }

        public class MonthlyBookingSummary
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public string WorkerName { get; set; }
            public int Count { get; set; }
        }

        public class BookingRoomDataContext
        {
            public static List<MonthlyBookingSummary> GetMonthlySummaries(DateTime selectedDate)
            {
                return records.Where(i => i.DateTime.Month == selectedDate.Month && i.DateTime.Year == selectedDate.Year)
                    .GroupBy(b => b.Client.Surname + " " + b.Client.Name + " " + b.Client.Patronymic)
                    .Select(g => new MonthlyBookingSummary
                    {
                        Year = selectedDate.Year,
                        Month = selectedDate.Month,
                        WorkerName = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(s => s.Count) // Сортировка по убыванию количества
                    /*.Take(5)*/ // Берем первые 5 самых популярных
                    .ToList();
            }
        }

        public class MainViewModel
        {
            public SeriesCollection Values { get; set; }
            public List<string> Labels { get; set; }

            public void LoadData(DateTime selectedDate)
            {
                var summaries = BookingRoomDataContext.GetMonthlySummaries(selectedDate);
                var values = new ChartValues<int>();
                Labels.Clear();
                Values.Clear(); // Очищаем предыдущие данные

                foreach (var summary in summaries)
                {
                    Labels.Add(summary.WorkerName.ToString()); // Добавляем название услуги в Labels
                    values.Add(summary.Count);
                }

                Values.Add(new ColumnSeries
                {
                    Title = "",
                    Values = values,
                    DataLabels = true, // Включаем подписи данных
                    LabelPoint = chartPoint => string.Format("{0:N0}", chartPoint.Y),
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6d584e")) // Задаем цвет колонок
                });
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DateDP.SelectedDate.HasValue)
            {
                DateTime selectedDate = DateDP.SelectedDate.Value;
                main.LoadData(selectedDate);
            }
        }

        private void PrintBTN_Click(object sender, RoutedEventArgs e)
        {
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog().GetValueOrDefault())
            {
                printDialog.PrintVisual(this, "Отчёт");
            }
            printDialog.PrintQueue.Dispose();
        }
    }
}

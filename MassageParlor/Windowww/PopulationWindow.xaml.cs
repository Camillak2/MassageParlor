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
    /// Логика взаимодействия для PopulationWindow.xaml
    /// </summary>
    public partial class PopulationWindow : Window
    {
        public static List<Record> records { get; set; }
        public static MainViewModel main { get; set; }

        public PopulationWindow()
        {
            InitializeComponent();
            records = DBConnection.massageSalon.Record.ToList();
            main = new MainViewModel();
            main.Values = new SeriesCollection();
            main.Labels = new List<string>();
            main.LoadData();
            DataContext = main.Values;
            AxesCollection axes = new AxesCollection();
            axes.Add(new Axis() { Labels = main.Labels });
            recordsChart.AxisX = axes;
        }

        public class MonthlyBookingSummary
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public string ServiceName { get; set; }
            public int Count { get; set; }
        }

        public class BookingRoomDataContext
        {
            public static List<MonthlyBookingSummary> GetMonthlySummaries()
            {
                return records.GroupBy(b => new { b.DateTime.Date.Year, b.DateTime.Date.Month, b.Service.Name })
                    .Select(g => new MonthlyBookingSummary
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        ServiceName = g.Key.Name,
                        Count = g.Count()
                    })
                    .OrderBy(s => s.Year).ThenBy(s => s.Month).ThenBy(s => s.ServiceName)
                    .ToList();
            }
        }

        public class MainViewModel
        {
            public SeriesCollection Values { get; set; }
            public List<string> Labels { get; set; }

            public void LoadData()
            {
                //var context = BookingRooms;
                var summaries = BookingRoomDataContext.GetMonthlySummaries();

                // Группировка по месяцам и услугам
                var groupedSummaries = summaries.GroupBy(s => new { s.Year, s.Month });

                foreach (var group in groupedSummaries)
                {
                    var monthName = new DateTime(group.Key.Year, group.Key.Month, 1).ToString("MMMMMMMM yyyy");
                    Labels.Add(monthName);

                    foreach (var summary in group)
                    {
                        // Создание серии данных для каждой услуги
                        Values.Add(new ColumnSeries
                        {
                            Title = summary.ServiceName, // Заголовок серии - название услуги
                            Values = new ChartValues<int> { summary.Count }, // Значение - количество бронирований
                            // ... (другие настройки серии)
                        });
                    }
                }
            }
        }
    }
}
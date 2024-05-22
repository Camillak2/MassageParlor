using LiveCharts;
using LiveCharts.Wpf;
using MassageParlor.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
    /// Логика взаимодействия для StatisticWindow.xaml
    /// </summary>
    public partial class StatisticWindow : Window
    {
        public static List<Record> records { get; set; }
        public static MainViewModel main { get; set; }

        public StatisticWindow()
        {
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
            public int TotalSum { get; set; }
        }

        public class BookingRoomDataContext
        {
            public static List<MonthlyBookingSummary> GetMonthlySummaries()
            {
                return records.GroupBy(b => new { b.DateTime.Year, b.DateTime.Month })
                    .Select(g => new MonthlyBookingSummary
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        TotalSum = g.Sum(b => (int)b.FinalPrice)
                    })
                    .OrderBy(s => s.Year).ThenBy(s => s.Month)
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

                var values = new ChartValues<int>();


                foreach (var summary in summaries)
                {
                    var monthName = new DateTime(summary.Year, summary.Month, 1).ToString("MMMMMMMM yyyy");
                    Labels.Add(monthName);
                    values.Add(summary.TotalSum);
                }

                Values.Add(new ColumnSeries
                {
                    Title = "Total Bookings",
                    Values = values,
                });
            }
        }
    }
}

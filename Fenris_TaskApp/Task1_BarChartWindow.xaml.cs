using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Windows;
using System.Windows.Media;

namespace Fenris_TaskApp
{
    public partial class Task1_BarChartWindow : Window
    {
        private Random _random;

        public Task1_BarChartWindow()
        {
            InitializeComponent();

            _random = new Random();

            for (int i = 0; i < 3; i++)
            {
                AddNewBar(_random.Next(1, 100));
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var value = _random.Next(1, 100);
            AddNewBar(value);
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (BarChart.Series.Count > 0)
            {
                BarChart.Series.RemoveAt(0);
            }
        }

        private void AddNewBar(double value)
        {
            var color = Color.FromRgb((byte)_random.Next(256), (byte)_random.Next(256), (byte)_random.Next(256));

            var series = new ColumnSeries
            {
                Values = new ChartValues<double> { value },
                Fill = new SolidColorBrush(color),
                Title = $"Bar {BarChart.Series.Count + 1}"
            };

            BarChart.Series.Add(series);
        }
    }
}

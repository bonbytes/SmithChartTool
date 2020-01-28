using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using OxyPlot;
using OxyPlot.Series;
using SmithChartTool.View;

namespace SmithChartTool.ViewModel
{
    public class MainWindowViewModel
    {
        public PlotModel MyFirstPlot { get; private set; }

        public MainWindowViewModel()
        {
            this.MyFirstPlot = new PlotModel { Title = "Plot" };
            this.MyFirstPlot.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));
        }

        public static RoutedUICommand CommandXYAsync = new RoutedUICommand("Run XY Async", "RXYA", typeof(MainWindowView), new InputGestureCollection() { new KeyGesture(Key.F5), new KeyGesture(Key.R, ModifierKeys.Control) });

        public async void RunCommandXYAsync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            await RunXYAsync();
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            MessageBox.Show(elapsedMs.ToString());
        }

        private async Task RunXYAsync()
        {
            var result = await Task.Run(() => 0) ;  // insert lambda body

            /// or (in case of parallel run)
            //List<Task<XYDataModel>> tasks = new List<Task<XYDataModel>>();
            //foreach (string data in websites)
            //{
            //    tasks.Add(RunXYAsync(data));
            //}
            //var results = await Task.WhenAll(tasks);
        }

    }

}

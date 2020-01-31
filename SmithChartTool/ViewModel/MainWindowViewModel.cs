using System;
using System.Collections.Generic;
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
    
        /*
        // konforme Abbdildung in Matlab / Octave
        f = @(x) (x - 1) ./ (x + 1);

        close all;
        hold on;

        zero_line = 1E-20

        jrange = logspace(log10(0.15), log10(100), 10)

        # imag. const
        for jmul = [-jrange zero_line jrange]
            plot(f((0:0.1:100)+(j*jmul)))
        endfor

        # real. const
        for roffs = [0 0.2 0.5 1 2 5]
            plot(f(roffs + j*(-100:0.05:100)))
        endfor 
        */

        public PlotModel MyFirstPlot { get; private set; }

        private MathNet.Numerics.Complex32 getConformalValue(MathNet.Numerics.Complex32 z)
        {
            return ((z - 1) / (z + 1));
        }

        public FunctionSeries DrawSmithChart()
        {
            List<double> x = new List<double>(new IEnumerable<double>());
            double i = 0;
            
            while(i <= 100)
            {
                x.Add(i);
                i = i + 0.1;
            }


            FunctionSeries serie = new FunctionSeries();

            for (int x = 0; x < n; x++)
            {
                for (int y = 0; y < n; y++)
                {
                    //adding the points based x,y
                    DataPoint data = new DataPoint(x, getValue(x, y));

                    //adding the point to the serie
                    serie.Points.Add(data);
                }
            }
            //returning the serie
            return serie;
        }

        public MainWindowViewModel()
        {
            this.MyFirstPlot = new PlotModel();
            this.MyFirstPlot.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));
            MyFirstPlot.LegendPosition = LegendPosition.RightBottom;
            MyFirstPlot.LegendPlacement = LegendPlacement.Outside;
            MyFirstPlot.LegendOrientation = LegendOrientation.Horizontal;
            var Yaxis = new OxyPlot.Axes.LinearAxis();
            OxyPlot.Axes.LinearAxis XAxis = new OxyPlot.Axes.LinearAxis { Position = OxyPlot.Axes.AxisPosition.Bottom, Minimum = -1, Maximum = 1 };
            XAxis.Title = "Real";
            Yaxis.Title = "Imaginary";
            MyFirstPlot.Axes.Add(Yaxis);
            MyFirstPlot.Axes.Add(XAxis);
            //this.plot.Model = MyFirstPlot;
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

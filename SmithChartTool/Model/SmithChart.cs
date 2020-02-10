using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using OxyPlot;
using OxyPlot.Series;

namespace SmithChartTool.Model
{
    public class SmithChart: INotifyPropertyChanged
    {
        public PlotModel Plot { get; private set; }
        public double Frequency { get; private set; }
        public ImpedanceElement ReferenceImpedance { get; private set; }
        public bool IsNormalized { get; private set; }
        public int NumRealCircles { get; private set; }
        public int NumImagCircles { get; private set; }

        private Complex32 GetConformalImpedanceValue(Complex32 z)
        {
            return ((z - 1) / (z + 1));
        }

        private List<double> GetLinRange(double start, double stop, int steps)
        {
            List<double> temp = new List<double>();
            for (int i = 0; i < steps; i++)
            {
                temp.Add(start + (stop - start) * ((double)i / (steps - 1)));
            }
            return temp;
            //return Enumerable.Range(0, steps).Select(i => start + (stop-start) * ((double)i / (steps-1))); // obsolete: Enumerable.Range only can return integer values...
        }

        private List<double> GetLogRange(double start, double stop, int steps)
        {
            double p = (stop - start) / (steps - 1);
            List<double> temp = new List<double>();
            for (int i = 0; i < steps; i++)
            {
                temp.Add(Math.Pow(10.0, start + p * i));
            }
            return temp;
        }

        //public List<LineSeries> DrawSmithChart(int numRealCirc = 7, int numImagCirc = 18)
        //{
        //    List<double> y = GetLinRange(0, 100, 1000);
        //    List<double> jrange = GetLogRange(Math.Log(0.15, 10), Math.Log(100, 10), 10);
        //    List<double> jrangeFull = new List<double>(jrange.Invert());

        //    jrangeFull.Add(1e-20);
        //    jrangeFull.AddRange(jrange);
        //    //List<List<Complex32>> imagConstValues = new List<List<Complex32>>();
        //    //List<List<DataPoint>> imagCurves = new List<List<DataPoint>>();

        //    List<LineSeries> series = new List<LineSeries>();

        //    int i = 0;
        //    foreach (var im in jrangeFull)
        //    {
        //        series.Add(new LineSeries { LineStyle = LineStyle.Dot });
        //        //imagConstValues.Add(new List<Complex32>());
        //        //imagCurves.Add(new List<DataPoint>());
        //        foreach (var re in y)
        //        {
        //            Complex32 _z = GetConformalImpedanceValue(new Complex32((float)re, (float)im));
        //            //imagConstValues[i].Add(_z); // every i represents one circle with constant imaginary part
        //            //imagCurves[i].Add(new DataPoint(_z.Real, _z.Imaginary));
        //            series[i].Points.Add(new DataPoint(_z.Real, _z.Imaginary));
        //        }
        //        i++;
        //    }

        //    List<double> rrangeFull = new List<double> {0, 0.2, 0.5, 1, 2, 5, 10};
        //    List<double> x = GetLogRange(-10, Math.Log(100,10), 1000);//GetRange(-100, 100, 4000);
        //    x.AddRange(x.Invert());
        //    //List<List<Complex32>> realConstValues = new List<List<Complex32>>();
        //    //List<List<DataPoint>> realCurves = new List<List<DataPoint>>();

        //    i = 0;
        //    foreach (var re in rrangeFull)
        //    {
        //        series.Add(new LineSeries { LineStyle = LineStyle.Solid });
        //        //realConstValues.Add(new List<Complex32>());
        //        //realCurves.Add(new List<DataPoint>());
        //        foreach (var im in x)
        //        {
        //            Complex32 _z = GetConformalImpedanceValue(new Complex32((float)re, (float)im));
        //            //_z = GetConformalValue(_z);
        //            //realConstValues[i].Add(_z);  // every i represents one circle with constant real part
        //            //realCurves[i].Add(new DataPoint(_z.Real, _z.Imaginary));
        //            series[i].Points.Add(new DataPoint(_z.Real, _z.Imaginary));
        //        }
        //        i++;
        //    }

        //    return series;
        //}

        public List<LineSeries> DrawSmithChartReal(int numRealCirc = 7)
        {
            List<LineSeries> series = new List<LineSeries>();
            List<double> rrangeFull = new List<double> { 0, 0.2, 0.5, 1, 2, 5, 10 };
            List<double> x = GetLogRange(-10, Math.Log(100, 10), 1000);//GetRange(-100, 100, 4000);
            x.AddRange(x.Invert());
            int i = 0;

            foreach (var re in rrangeFull)
            {
                series.Add(new LineSeries { LineStyle = LineStyle.Solid });
                foreach (var im in x)
                {
                    Complex32 _z = GetConformalImpedanceValue(new Complex32((float)re, (float)im));
                    series[i].Points.Add(new DataPoint(_z.Real, _z.Imaginary));
                }
                i++;
            }
            return series;
        }

        public List<LineSeries> DrawSmithChartImag(int numImagCirc = 18)
        {
            List<double> y = GetLinRange(0, 100, 1000);
            List<double> jrange = GetLogRange(Math.Log(0.15, 10), Math.Log(100, 10), 10);
            List<double> jrangeFull = new List<double>(jrange.Invert());
            jrangeFull.Add(1e-20);
            jrangeFull.AddRange(jrange);
            List<LineSeries> series = new List<LineSeries>();

            int i = 0;
            foreach (var im in jrangeFull)
            {
                series.Add(new LineSeries { LineStyle = LineStyle.Dot });
                foreach (var re in y)
                {
                    Complex32 _z = GetConformalImpedanceValue(new Complex32((float)re, (float)im));
                    series[i].Points.Add(new DataPoint(_z.Real, _z.Imaginary));
                }
                i++;
            }
            return series;
        }

        private void Init()
        {
            NumRealCircles = 7;
            NumImagCircles = 18;
            Plot.IsLegendVisible = false;
            Plot.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = OxyPlot.Axes.AxisPosition.Left, Minimum = -1, Maximum = 1, IsZoomEnabled = false, Title = "Imaginary" });
            Plot.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = OxyPlot.Axes.AxisPosition.Bottom, Minimum = -1, Maximum = 1, IsZoomEnabled = false, Title = "Real" });
            Plot.DefaultColors = new List<OxyColor> { (OxyColors.Black) };

            List<LineSeries> seriesReal = DrawSmithChartReal(this.NumRealCircles);
            foreach (var item in seriesReal)
            {
                Plot.Series.Add(item);
            }
            List<LineSeries> seriesImag = DrawSmithChartImag(this.NumImagCircles);
            foreach (var item in seriesImag)
            {
                Plot.Series.Add(item);
            }
        }

        private void Invalidate()
        {
            Plot.InvalidatePlot(true);
        }

        public SmithChart()
        {
            Frequency = 1.0e9;
            ReferenceImpedance = new ImpedanceElement(new Complex32(50, 0));
            IsNormalized = true;
            Plot = new PlotModel();

            Init();
            Invalidate();
        }

        #region INotifyPropertyChanged Members  
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}

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
        public double Frequency { get; set; }
        public ImpedanceElement ReferenceImpedance { get; private set; }
        public bool IsNormalized { get; set; }
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

        public List<MyLineSeries> DrawSmithChartRealCircles(int numRealCirc = 7)
        {
            List<MyLineSeries> series = new List<MyLineSeries>();
            List<double> rrangeFull = new List<double> { 0, 0.2, 0.5, 1, 2, 5, 10 };
            List<double> x = GetLogRange(-10, 4, 1000);//GetRange(-100, 100, 4000);
            //List<List<Complex32>> realConstValues = new List<List<Complex32>>();
            x.AddRange(x.Invert());
            int i = 0;

            foreach (var re in rrangeFull)
            {
                series.Add(new MyLineSeries { LineStyle = LineStyle.Solid, StrokeThickness = 0.75 });
                foreach (var im in x)
                {
                    Complex32 _z = GetConformalImpedanceValue(new Complex32((float)re, (float)im));
                    series[i].Points.Add(new DataPoint(_z.Real, _z.Imaginary));
                }
                i++;
            }
            return series;
        }

        public List<MyLineSeries> DrawSmithChartImagCircles(int numImagCirc = 18)
        {
            List<double> y = GetLinRange(0, 100, 1000);
            List<double> jrange = GetLogRange(Math.Log(0.15, 10), Math.Log(100, 10), 10);
            List<double> jrangeFull = new List<double>(jrange.Invert());
            jrangeFull.Add(1e-20);
            jrangeFull.AddRange(jrange);
            List<MyLineSeries> series = new List<MyLineSeries>();
            //List<List<Complex32>> imagConstValues = new List<List<Complex32>>();

            int i = 0;
            foreach (var im in jrangeFull)
            {
                series.Add(new MyLineSeries { LineStyle = LineStyle.Dash, StrokeThickness = 0.75 });
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
            Plot.Axes.Add(new OxyPlot.Axes.LinearAxis 
            { 
                Position = OxyPlot.Axes.AxisPosition.Left, 
                Minimum = -1, 
                Maximum = 1, 
                AbsoluteMaximum = 1, 
                AbsoluteMinimum = -1, 
                IsZoomEnabled = true, 
                Title = "Imaginary", 
                IsPanEnabled = true
            });
            Plot.Axes.Add(new OxyPlot.Axes.LinearAxis 
            { 
                Position = OxyPlot.Axes.AxisPosition.Bottom, 
                Minimum = -1, 
                Maximum = 1, 
                AbsoluteMaximum = 1, 
                AbsoluteMinimum = -1, 
                IsZoomEnabled = true, 
                Title = "Real", 
                IsPanEnabled= true
            });
            Plot.DefaultColors = new List<OxyColor> { (OxyColors.Black) };

            List<MyLineSeries> seriesReal = DrawSmithChartRealCircles(this.NumRealCircles);
            foreach (var item in seriesReal)
            {
                Plot.Series.Add(item);
            }
            List<MyLineSeries> seriesImag = DrawSmithChartImagCircles(this.NumImagCircles);
            foreach (var item in seriesImag)
            {
                Plot.Series.Add(item);
            }
        }

        private void Invalidate()
        {
            Plot.InvalidatePlot(true);
        }

        public void ExportImage()
        {
            
        }

        public SmithChart()
        {
            Frequency = 1.0e3;
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

    public class MyLineSeries : LineSeries
    {

        //List<ScreenPoint> outputBuffer = null;

        public bool Aliased { get; set; } = true;

        //protected override void RenderLine(IRenderContext rc, OxyRect clippingRect, IList<ScreenPoint> pointsToRender)
        //{
        //    var dashArray = this.ActualDashArray;

        //    if (this.outputBuffer == null)
        //    {
        //        this.outputBuffer = new List<ScreenPoint>(pointsToRender.Count);
        //    }

        //    rc.DrawClippedLine(clippingRect,
        //                       pointsToRender,
        //                       this.MinimumSegmentLength * this.MinimumSegmentLength,
        //                       this.GetSelectableColor(this.ActualColor),
        //                       this.StrokeThickness,
        //                       dashArray,
        //                       this.LineJoin,
        //                       this.Aliased,  // <-- this is the issue
        //                       this.outputBuffer);

        //}
    }
}

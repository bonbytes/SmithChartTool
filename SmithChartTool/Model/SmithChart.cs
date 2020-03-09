using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;

namespace SmithChartTool.Model
{
    public class SmithChart: INotifyPropertyChanged
    {
        public PlotModel Plot { get; private set; }
        private double _frequency;
        public double Frequency
        {
            get
            {
                return _frequency;
            }
            set
            {
                if (value != _frequency)
                {
                        _frequency = value;
                        OnPropertyChanged("Frequency");
                }
            }
        }
        public ImpedanceElement ReferenceImpedance { get; set; }
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

        public List<MyLineSeries> DrawSmithChartImagCircles(int numImagCircSym = 7)
        {
            List<MyLineSeries> series = new List<MyLineSeries>();
            List<double> rrangeFull = new List<double> { 0, 0.2, 0.5, 1, 2, 5, 10 };
            List<double> x = GetLogRange(-10, 4, 1000);
            var temp = x.Invert();
            var temp2 = x;
            temp2.Reverse();
            temp.AddRange(temp2);
            x = temp;
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

        public List<MyLineSeries> DrawSmithChartRealCircles(int numRealCirc = 13)
        {
            List<double> y = GetLinRange(0, 100, 1000);
            List<double> jrange = GetLogRange(Math.Log(0.15, 10), Math.Log(1000, 10), numRealCirc);
            List<double> jrangeFull = new List<double>(jrange.Invert());
            jrangeFull.Add(1e-20);
            jrangeFull.AddRange(jrange);
            List<MyLineSeries> series = new List<MyLineSeries>();
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
            NumRealCircles = 13;
            NumImagCircles = 7;
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

            List<MyLineSeries> seriesReal = DrawSmithChartImagCircles(this.NumRealCircles);
            foreach (var item in seriesReal)
            {
                Plot.Series.Add(item);
            }
            List<MyLineSeries> seriesImag = DrawSmithChartRealCircles(this.NumImagCircles);
            foreach (var item in seriesImag)
            {
                Plot.Series.Add(item);
            }
            Plot.Annotations.Add(new TextAnnotation() { Text = "Blub", TextPosition = new DataPoint(0.2, -0.5) });
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

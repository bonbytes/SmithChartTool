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
        public int NumResistanceCircles { get; private set; }
        public int NumReactanceCircles { get; private set; }
        public int NumConductanceCircles { get; private set; }
        public int NumSusceptanceCircles { get; private set; }

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

        public List<MyLineSeries> DrawSmithChartConstResistanceCircles(int numResistanceCirc = 7)
        {
            List<MyLineSeries> series = new List<MyLineSeries>();
            List<double> rrangeFull = new List<double> { 0, 0.2, 0.5, 1, 2, 5, 10 }; // 7 circles
            List<double> x = GetLogRange(Math.Log(1E-10, 10), Math.Log(100, 10), 1000); // number of points per circle
            var temp = x.Invert();
            var temp2 = x;
            temp2.Reverse();
            temp.AddRange(temp2);
            x = temp;
            int i = 0;

            foreach (var re in rrangeFull) // for every constant resistance circle
            {
                series.Add(new MyLineSeries { LineStyle = LineStyle.Solid, StrokeThickness = 0.75 });
                foreach (var im in x) // plot single circle through conformal mapping
                {
                    Complex32 _z = GetConformalImpedanceValue(new Complex32((float)re, (float)im));
                    series[i].Points.Add(new DataPoint(_z.Real, _z.Imaginary));
                }
                i++;
            }
            return series;
        }

        public List<MyLineSeries> DrawSmithChartConstReactanceCircles(int numReactanceCirc = 12)
        {
            List<double> y = GetLogRange(-5, 100, 1000); // value of R -> zero to infinity
            List<double> jrange = GetLogRange(Math.Log(0.15, 10), Math.Log(1000, 10), numReactanceCirc);
            List<double> jrangeFull = new List<double>(jrange.Invert());
            jrangeFull.Add(1e-20);  // "zero" line
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

        public void DrawSmithChartLegend()
        {
            Plot.Annotations.Add(new TextAnnotation() { Text = "Blub", TextPosition = new DataPoint(0.2, -0.5) });

        }

        private void Init()
        {
            NumResistanceCircles = 7;
            NumReactanceCircles = 10;
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

            List<MyLineSeries> seriesResistanceCircles = DrawSmithChartConstResistanceCircles(NumResistanceCircles);
            foreach (var item in seriesResistanceCircles)
            {
                Plot.Series.Add(item);
            }
            List<MyLineSeries> seriesReactanceCircles = DrawSmithChartConstReactanceCircles(NumReactanceCircles);
            foreach (var item in seriesReactanceCircles)
            {
                Plot.Series.Add(item);
            }

            var series1 = new LineSeries();
            series1.Color = OxyColor.FromRgb(22, 22, 22);
            series1.StrokeThickness = 1;
            series1.MarkerType = MarkerType.Diamond;
            series1.MarkerStroke = OxyColors.Blue;
            series1.MarkerFill = OxyColors.Beige;
            series1.MarkerStrokeThickness = 5;
            series1.MarkerSize = 2;
            series1.DataFieldX = "Date";
            series1.DataFieldY = "Value";
            series1.TrackerFormatString = "Date: {2:d HH}&#x0a;Value: {4}";

            series1.Points.Add(new DataPoint(-0.1, 0.3));
            series1.Points.Add(new DataPoint(0.2, 0.4));
            series1.Points.Add(new DataPoint(0.4, 0.5));
            series1.Points.Add(new DataPoint(0.6, 0.6));

            Invalidate();
        }

        public void Invalidate()
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

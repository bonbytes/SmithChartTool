using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public const double c0 = 299792458;  // speed of light in m/s

        public PlotModel Plot { get; private set; }
        public ImpedanceElement ReferenceImpedance { get; set; }
        public bool IsNormalized { get; set; }
        public int NumResistanceCircles { get; private set; }
        public int NumReactanceCircles { get; private set; }
        public int NumConductanceCircles { get; private set; }
        public int NumSusceptanceCircles { get; private set; }

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

        public ObservableCollection<Complex32> _inputImpedances;
        public ObservableCollection<Complex32> InputImpedances
        {
            get { return _inputImpedances; }
            set
            {
                _inputImpedances = value;
                OnPropertyChanged("InputImpedances");
            }
        }

        public static double FrequencyToWavelength(double frequency)
        {
            return c0 / frequency;
        }

        public static double WavelengthToFrequency(double wavelength)
        {
            return c0 / wavelength;
        }

        public static double CalculatePropagationConstant(double wavelength)
        {
            return (2 * Math.PI) / (wavelength);
        }

        public float CalculateSerialCapacitorReactance(double value, double frequency)
        {
            return (float)(1 / (2 * Math.PI * frequency * value));
        }

        public float CalculateSerialInductorReactance(double value, double frequency)
        {
            return (float)(2 * Math.PI * frequency * value);
        }

        private static Complex32 GetConformalGammaValue(Complex32 impedance)
        {
            return ((impedance - 1) / (impedance + 1));
        }

        private static Complex32 GetConformalImpedanceValue(Complex32 gamma)
        {
            return ((gamma + 1) / (gamma - 1));
        }
        public static double CalculateVSWR(Complex32 gamma)
        {
            return ((1 + gamma.Magnitude) / (1 - gamma.Magnitude));
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
                    Complex32 _z = GetConformalGammaValue(new Complex32((float)re, (float)im));
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
                    Complex32 _z = GetConformalGammaValue(new Complex32((float)re, (float)im));
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
            InvalidatePlot();
            AddTestMarker();
        }

        public void AddTestMarker()
        {
            var series1 = new LineSeries();
            series1.Color = OxyColor.FromRgb(22, 22, 22);
            series1.StrokeThickness = 2;
            series1.MarkerType = MarkerType.Diamond;
            series1.MarkerStroke = OxyColors.Blue;
            series1.MarkerFill = OxyColors.Beige;
            series1.MarkerStrokeThickness = 3;
            series1.MarkerSize = 3;

            series1.Points.Add(new DataPoint(-0.1, 0.3));
            series1.Points.Add(new DataPoint(0.2, 0.4));
            series1.Points.Add(new DataPoint(0.4, 0.5));
            series1.Points.Add(new DataPoint(0.6, 0.6));

            Plot.Series.Add(series1);
            InvalidatePlot();
        }

        public void InvalidatePlot()
        {
            Plot.InvalidatePlot(true);
        }

        public void ExportImage()
        {
            
        }

        public void InvalidateInputImpedances(Schematic schematic)
        {
            InputImpedances.Clear();
            Complex32 transformationImpedance;

            for (int i = schematic.Elements.Count - 1; i > 0; i--)
            {
                if (schematic.Elements[i].Type == SchematicElementType.Port)
                    InputImpedances.Add(schematic.Elements[i].Impedance);
                else
                {
                    switch (schematic.Elements[i].Type)
                    {
                        case SchematicElementType.ResistorSerial:
                            transformationImpedance = new Complex32((float)schematic.Elements[i].Value, 0);
                            break;
                        case SchematicElementType.CapacitorSerial:
                            transformationImpedance = new Complex32(0, CalculateSerialCapacitorReactance(schematic.Elements[i].Value, Frequency));
                            break;
                        case SchematicElementType.InductorSerial:
                            transformationImpedance = new Complex32(0, CalculateSerialInductorReactance(schematic.Elements[i].Value, Frequency));
                            break;
                        case SchematicElementType.ResistorParallel:
                            transformationImpedance = 0;
                            break;
                        case SchematicElementType.CapacitorParallel:
                            transformationImpedance = 0;
                            break;
                        case SchematicElementType.InductorParallel:
                            transformationImpedance = 0;
                            break;
                        case SchematicElementType.TLine:
                            transformationImpedance = 0;
                            break;
                        case SchematicElementType.OpenStub:
                            transformationImpedance = 0;
                            break;
                        case SchematicElementType.ShortedStub:
                            transformationImpedance = 0;
                            break;
                        case SchematicElementType.ImpedanceSerial:
                            transformationImpedance = schematic.Elements[i].Impedance;
                            break;
                        case SchematicElementType.ImpedanceParallel:
                            transformationImpedance = 0;
                            break;
                        default:
                            transformationImpedance = 0;
                            throw new NotImplementedException("Invalid transformation. Aborting...");
                    }
                    InputImpedances.Add(InputImpedances.Last() + transformationImpedance);
                }
            }
            OnPropertyChanged("InputImpedances");
        }

        public SmithChart()
        {
            Frequency = 1.0e9;
            ReferenceImpedance = new ImpedanceElement(new Complex32(50, 0));
            IsNormalized = true;
            Plot = new PlotModel();
            InputImpedances = new ObservableCollection<Complex32>();

            Init();
            InvalidatePlot();
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

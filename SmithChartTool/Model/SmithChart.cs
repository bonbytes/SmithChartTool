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
        public enum SmithChartType
        {
            Impedance,
            Admittance
        }

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

        public ObservableCollection<InputImpedance> _inputImpedances;
        public ObservableCollection<InputImpedance> InputImpedances
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

        public static Complex32 CalculateSerialResistorResistance(double value)
        {
            return new Complex32((float)value, 0);
        }

        public static Complex32 CalculateParallelResistorConductance(double value)
        {
            return new Complex32((float)(1/value), 0);
        }

        public static Complex32 CalculateSerialCapacitorReactance(double value, double frequency)
        {
            return new Complex32(0, (float)(1 / (2 * Math.PI * frequency * value)));
        }

        public static Complex32 CalculateSerialInductorReactance(double value, double frequency)
        {
            return new Complex32(0, (float)(2 * Math.PI * frequency * value));
        }

        public static Complex32 CalculateParallelCapacitorSusceptance(double value, double frequency)
        {
            return new Complex32(0, (float)(2 * Math.PI * frequency * value));
        }

        public static Complex32 CalculateParallelInductorSusceptance(double value, double frequency)
        {
            return new Complex32(0, (float)(1 / (2 * Math.PI * frequency * value)));
        }

        public static Complex32 GetConformalGammaValue(Complex32 input, SmithChartType type)
        {
            if (type == SmithChartType.Impedance)
                return ((input - 1) / (input + 1));
            else if (type == SmithChartType.Admittance)
                return -((input - 1) / (input + 1));
            else
                return 0;
        }

        public static double CalculateVSWR(Complex32 gamma)
        {
            return ((1 + gamma.Magnitude) / (1 - gamma.Magnitude));
        }

        public static List<double> GetLinRange(double start, double stop, int steps)
        {
            List<double> temp = new List<double>();
            for (int i = 0; i < steps; i++)
            {
                temp.Add(start + (stop - start) * ((double)i / (steps - 1)));
            }
            return temp;
            //return Enumerable.Range(0, steps).Select(i => start + (stop-start) * ((double)i / (steps-1))); // obsolete: Enumerable.Range only can return integer values...
        }

        public static List<double> GetLogRange(double start, double stop, int steps)
        {
            // call with: GetLogRange(Math.Log(MinValue, 10), Math.Log(MaxValue, 10), NumberOfPoints);
            double p = (stop - start) / (steps - 1);
            List<double> temp = new List<double>();
            for (int i = 0; i < steps; i++)
            {
                temp.Add(Math.Pow(10.0, start + p * i));
            }
            return temp;
        }

        private void DrawConstRealCircles(SmithChartType type)
        {
            List<MyLineSeries> series = new List<MyLineSeries>();
            List<double> reRangeFull = new List<double> { 0, 0.2, 0.5, 1, 2, 5, 10, 50};
            List<double> values = GetLogRange(Math.Log(1e-6, 10), Math.Log(1e6, 10), 250); // imaginary value of every circle
            var temp = values.Invert();
            var temp2 = values;
            temp2.Reverse();
            temp.AddRange(temp2);
            values = temp;
            int i = 0;

            foreach (var re in reRangeFull) // for every real const circle
            {
                series.Add(new MyLineSeries { LineStyle = LineStyle.Solid, StrokeThickness = 0.75 });
                foreach (var im in values) // plot every single circle through conformal mapping
                {
                    Complex32 r = GetConformalGammaValue(new Complex32((float)re, (float)im), type);
                    series[i].Points.Add(new DataPoint(r.Real, r.Imaginary));
                }
                Plot.Series.Add(series[i]);
                i++;
            }
        }

        private void DrawConstImaginaryCircles(SmithChartType type)
        {
            List<double> values = GetLogRange(Math.Log(1e-6, 10), Math.Log(1e6, 10), 250); // real value of every circle
            List<double> imRange = new List<double>() { 0.2, 0.5, 1, 2, 5, 10, 20, 50 };
            List<double> imRangeFull = new List<double>(imRange.Invert());
            imRangeFull.Add(1e-20);  // "zero" line
            imRangeFull.AddRange(imRange);
            List<MyLineSeries> series = new List<MyLineSeries>();
            int i = 0;
            foreach (var im in imRangeFull)
            {
                series.Add(new MyLineSeries { LineStyle = LineStyle.Dash, StrokeThickness = 0.75 });
                foreach (var re in values) // plot every single circle through conformal mapping
                {
                    Complex32 r = GetConformalGammaValue(new Complex32((float)re, (float)im), type);
                    series[i].Points.Add(new DataPoint(r.Real, r.Imaginary));
                }
                Plot.Series.Add(series[i]);
                i++;
            }
        }

        public void DrawLegend()
        {
            Plot.Annotations.Add(new TextAnnotation() { Text = "Blub", TextPosition = new DataPoint(0.2, -0.5) });
        }

        public void Draw(SmithChartType type)
        {
            DrawConstRealCircles(type);
            DrawConstImaginaryCircles(type);
        }

        private void Init()
        {
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

            Draw(SmithChartType.Impedance);
            Invalidate();
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
            Invalidate();
        }

        public void Invalidate()
        {
            Plot.InvalidatePlot(true);
        }

        public void ExportImage()
        {
            
        }

        public void InvalidateInputImpedances(Schematic schematic)
        {
            InputImpedances.Clear();
            Complex32 transformer;

            for (int i = schematic.Elements.Count - 1; i > 0; i--)
            {
                if (schematic.Elements[i].Type == SchematicElementType.Port)
                    InputImpedances.Add(new InputImpedance(i, schematic.Elements[i].Impedance));
                else
                {
                    switch (schematic.Elements[i].Type)
                    {
                        case SchematicElementType.ResistorSerial:
                            transformer = CalculateSerialResistorResistance(schematic.Elements[i].Value);
                            break;
                        case SchematicElementType.CapacitorSerial:
                            transformer = CalculateSerialCapacitorReactance(schematic.Elements[i].Value, Frequency);
                            break;
                        case SchematicElementType.InductorSerial:
                            transformer = CalculateSerialInductorReactance(schematic.Elements[i].Value, Frequency);
                            break;
                        case SchematicElementType.ResistorParallel:
                            transformer = CalculateParallelResistorConductance(schematic.Elements[i].Value);
                            break;
                        case SchematicElementType.CapacitorParallel:
                            transformer = CalculateParallelCapacitorSusceptance(schematic.Elements[i].Value, Frequency);
                            break;
                        case SchematicElementType.InductorParallel:
                            transformer = CalculateParallelInductorSusceptance(schematic.Elements[i].Value, Frequency);
                            break;
                        case SchematicElementType.TLine:
                            transformer = 0;
                            break;
                        case SchematicElementType.OpenStub:
                            transformer = new Complex32(0, -(schematic.Elements[i].Impedance.Real * (float)Math.Tan(schematic.Elements[i].Value)));
                            break;
                        case SchematicElementType.ShortedStub:
                            transformer = new Complex32(0, (schematic.Elements[i].Impedance.Real * (float)Math.Tan(schematic.Elements[i].Value)));
                            break;
                        case SchematicElementType.ImpedanceSerial:
                            transformer = schematic.Elements[i].Impedance;
                            break;
                        case SchematicElementType.ImpedanceParallel:
                            transformer = 1/(schematic.Elements[i].Impedance);
                            break;
                        default:
                            transformer = 0;
                            break;
                    }
                    switch (schematic.Elements[i].Type)
                    {
                        case SchematicElementType.ResistorSerial:
                        case SchematicElementType.CapacitorSerial:
                        case SchematicElementType.InductorSerial:
                        case SchematicElementType.ImpedanceSerial:
                            InputImpedances.Add(new InputImpedance(i, InputImpedances.Last().Impedance + transformer));
                            break;
                        case SchematicElementType.ResistorParallel:
                        case SchematicElementType.CapacitorParallel:
                        case SchematicElementType.InductorParallel:
                        case SchematicElementType.ImpedanceParallel:
                        case SchematicElementType.OpenStub:
                        case SchematicElementType.ShortedStub:
                            InputImpedances.Add(new InputImpedance(i, 1/(1/InputImpedances.Last().Impedance + 1/transformer)));
                            break;
                        case SchematicElementType.TLine:
                            InputImpedances.Add(new InputImpedance(i, 0));
                            //float z1 = schematic.Elements[i].Impedance.Real * (float)Math.Tan(schematic.Elements[i].Value);
                            //Complex32 z2 = Complex32.Multiply(InputImpedances.Last(), (Complex32)Trig.Tan(schematic.Elements[i].Value));
                            //InputImpedances.Add( Complex32.Multiply(schematic.Elements[i].Impedance.Real ,((Complex32.Add(InputImpedances.Last(), z1)) / (Complex32.Add(schematic.Elements[i].Impedance, z2)))));
                            break;
                        default:
                            break;
                    }
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
            InputImpedances = new ObservableCollection<InputImpedance>();

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

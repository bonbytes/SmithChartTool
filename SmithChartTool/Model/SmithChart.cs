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
using SmithChartTool.Utility;

namespace SmithChartTool.Model
{
    public class SmithChart
    {
        public enum SmithChartType
        {
            Impedance,
            Admittance
        }

        public PlotModel Plot { get; private set; }
        public LineSeries MarkerSeries { get; set; }
        public List <SCTLineSeries> IntermediateCurveSeries { get; set; }

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
                }
            }
        }
        private ImpedanceElement _referenceImpedance;
        public ImpedanceElement ReferenceImpedance
        {
            get
            {
                return _referenceImpedance;
            }
            set
            {
                if (value != _referenceImpedance)
                {
                    _referenceImpedance = value;
                }
            }
        }
        private bool _isNormalized;
        public bool IsNormalized
        {
            get
            {
                return _isNormalized;
            }
            set
            {
                if (value != _isNormalized)
                {
                    _isNormalized = value;
                }
            }
        }

        public int NumResistanceCircles { get; private set; }
        public int NumReactanceCircles { get; private set; }
        public int NumConductanceCircles { get; private set; }
        public int NumSusceptanceCircles { get; private set; }

        public Complex32 GetConformalGammaValue(Complex32 input, SmithChartType inputType, bool isNormalized)
        {
            if(isNormalized == false)
            {
                input = input / ReferenceImpedance.Impedance;
            }
            if (inputType == SmithChartType.Impedance)
                return ((input - 1) / (input + 1));
            else if (inputType == SmithChartType.Admittance)
                return -((input - 1) / (input + 1));
            else
                return 0;
        }

        private void DrawConstRealCircles(SmithChartType type)
        {
            List<SCTLineSeries> series = new List<SCTLineSeries>();
            List<double> reRangeFull = new List<double> { 0, 0.2, 0.5, 1, 2, 5, 10, 50};
            List<double> values = Lists.GetLogRange(Math.Log(1e-6, 10), Math.Log(1e6, 10), 500); // imaginary value of every circle
            var temp = values.Invert();
            var temp2 = values;
            temp2.Reverse();
            temp.AddRange(temp2);
            values = temp;
            int i = 0;

            foreach (var re in reRangeFull) // for every real const circle
            {
                series.Add(new SCTLineSeries { LineStyle = LineStyle.Solid, StrokeThickness = 0.75 });
                foreach (var im in values) // plot every single circle through conformal mapping
                {
                    Complex32 r = GetConformalGammaValue(new Complex32((float)re, (float)im), type, true);
                    series[i].Points.Add(new DataPoint(r.Real, r.Imaginary));
                }
                Plot.Series.Add(series[i]);
                i++;
            }
        }

        private void DrawConstImaginaryCircles(SmithChartType type)
        {
            List<double> values = Lists.GetLogRange(Math.Log(1e-6, 10), Math.Log(1e6, 10), 1000); // real value of every circle
            List<double> imRange = new List<double>() { 0.2, 0.5, 1, 2, 5, 10, 20, 50 };
            List<double> imRangeFull = new List<double>(imRange.Invert());
            imRangeFull.Add(1e-20);  // "zero" line
            imRangeFull.AddRange(imRange);
            List<SCTLineSeries> series = new List<SCTLineSeries>();
            int i = 0;
            foreach (var im in imRangeFull)
            {
                series.Add(new SCTLineSeries { LineStyle = LineStyle.Dash, StrokeThickness = 0.75 });
                foreach (var re in values) // plot every single circle through conformal mapping
                {
                    Complex32 r = GetConformalGammaValue(new Complex32((float)re, (float)im), type, true);
                    series[i].Points.Add(new DataPoint(r.Real, r.Imaginary));
                }
                Plot.Series.Add(series[i]);
                i++;
            }
        }

        public Complex32 ImpedanceNormalized(Complex32 impedance)
        {
            if (IsNormalized == true)
                return (impedance / ReferenceImpedance.Impedance);
            return impedance;
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

        private void Invalidate()
        {
            Plot.InvalidatePlot(true);
        }

        private void ClearMarkers()
        {
            MarkerSeries.Points.Clear();
            Invalidate();
        }

        private void ClearIntermediateCurves()
        {
            foreach (var series in IntermediateCurveSeries)
            {
                Plot.Series.Remove(series);
            }
            IntermediateCurveSeries.Clear();

            Invalidate();
        }

        private void AddMarker(Complex32 impedance)
        {
            Complex32 gamma = GetConformalGammaValue(impedance, SmithChartType.Impedance, IsNormalized);
            MarkerSeries.Points.Add(new DataPoint(gamma.Real, gamma.Imaginary));
        }

        private void AddIntermediateCurves(Complex32 impedanceNew, Complex32 impedanceOld, int numberOfPoints = 100)
        {
            SCTLineSeries series = new SCTLineSeries { LineStyle = LineStyle.Solid, Color = OxyColor.FromRgb(50, 50, 50), StrokeThickness = 4 };

            Complex32 gamma = new Complex32();
            List <double> listReal= Lists.GetLinRange(impedanceOld.Real, impedanceNew.Real, numberOfPoints);
            List <double> listImaginary = Lists.GetLinRange(impedanceOld.Imaginary, impedanceNew.Imaginary, numberOfPoints);

            for ( int i = 0; i < numberOfPoints; i++)
            {
                gamma = GetConformalGammaValue(new Complex32((float)listReal[i], (float)listImaginary[i]), SmithChartType.Impedance, IsNormalized);
                series.Points.Add(new DataPoint(gamma.Real, gamma.Imaginary));
            }
            IntermediateCurveSeries.Add(series);
        }

        public void InvalidateMarkers(ObservableCollection<InputImpedance> inputImpedances)
        {
            ClearMarkers();
            ClearIntermediateCurves();
            for (int i = 0; i<inputImpedances.Count; i++)
            {
                if (i > 0)
                    AddIntermediateCurves(inputImpedances[i].Impedance, inputImpedances[i - 1].Impedance);
                AddMarker(inputImpedances[i].Impedance);  
            }
            foreach (var series in IntermediateCurveSeries)
            {
                Plot.Series.Add(series);
            }
            Invalidate();
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
                IsPanEnabled = true
            });
            Plot.DefaultColors = new List<OxyColor> { (OxyColors.Black) };

            MarkerSeries = new LineSeries();
            MarkerSeries.Color = OxyColor.FromRgb(22, 22, 22);
            MarkerSeries.StrokeThickness = 0;
            MarkerSeries.MarkerType = MarkerType.Diamond;
            MarkerSeries.MarkerStroke = OxyColors.BlueViolet;
            MarkerSeries.MarkerFill = OxyColors.Beige;
            MarkerSeries.MarkerStrokeThickness = 3;
            MarkerSeries.MarkerSize = 5;
            Plot.Series.Add(MarkerSeries);

            IntermediateCurveSeries = new List <SCTLineSeries>();

            Draw(SmithChartType.Impedance);
            Invalidate();
        }

        public SmithChart()
        {
            Frequency = 1.0e9;
            ReferenceImpedance = new ImpedanceElement(new Complex32(50, 0));
            IsNormalized = false;
            Plot = new PlotModel();

            Init();
            Invalidate();
        }
    }
}

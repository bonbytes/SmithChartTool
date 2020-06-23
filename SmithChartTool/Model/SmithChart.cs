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
    public enum SmithChartType
    {
        Impedance,
        Admittance
    }

    public class SmithChart
    {
        public List<SCTLineSeries> ConstRealImpedanceCircleSeries { get; set; }
        public List<SCTLineSeries> ConstImaginaryImpedanceCircleSeries { get; set; }
        public List<SCTLineSeries> ConstRealAdmittanceCircleSeries { get; set; }
        public List<SCTLineSeries> ConstImaginaryAdmittanceCircleSeries { get; set; }
        public SCTLineSeries RefMarkerSeries { get; set; }
        public SCTLineSeries MarkerSeries { get; set; }
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
                    if (!(double.TryParse(value.ToString(), out _frequency)))
                        throw new ArgumentException("Given frequency is invalid", "Frequency");
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
                    if (!(bool.TryParse(value.ToString(), out _isNormalized)))
                        throw new ArgumentException("Only true or false allowed", "IsNormalized");
                }
            }
        }
        private bool _isImpedanceSmithChart;
        public bool IsImpedanceSmithChart
        {
            get
            {
                return _isImpedanceSmithChart;
            }
            set
            {
                if (value != _isImpedanceSmithChart)
                {
                    _isImpedanceSmithChart = value;
                    OnSmithChartChanged();
                }
            }
        }
        private bool _isAdmittanceSmithChart;
        public bool IsAdmittanceSmithChart
        {
            get
            {
                return _isAdmittanceSmithChart;
            }
            set
            {
                if (value != _isAdmittanceSmithChart)
                {
                    _isAdmittanceSmithChart = value;
                    OnSmithChartChanged();
                }
            }
        }

        public int NumResistanceCircles { get; private set; }
        public int NumReactanceCircles { get; private set; }
        public int NumConductanceCircles { get; private set; }
        public int NumSusceptanceCircles { get; private set; }

        private void CalculateConstRealCircles(SmithChartType type)
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
                if (type == SmithChartType.Impedance)
                    ConstRealImpedanceCircleSeries.Add(series[i]);
                else if (type == SmithChartType.Admittance)
                    ConstRealAdmittanceCircleSeries.Add(series[i]);
                else
                    throw new ArgumentException("Wrong SmithChart Type", "type");
                i++;
            }
        }

        private void CalculateConstImaginaryCircles(SmithChartType type)
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
                if (type == SmithChartType.Impedance)
                    ConstImaginaryImpedanceCircleSeries.Add(series[i]);
                else if (type == SmithChartType.Admittance)
                    ConstImaginaryAdmittanceCircleSeries.Add(series[i]);
                else
                    throw new ArgumentException("Wrong SmithChart Type", "type");
                i++;
            }
        }

        private void CalculateCircles(SmithChartType type)
        {
            CalculateConstRealCircles(type);
            CalculateConstImaginaryCircles(type);
        }

        public Complex32 GetConformalGammaValue(Complex32 input, SmithChartType inputType, bool isNormalized)
        {
            if (isNormalized == false)
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

        public Complex32 ImpedanceNormalized(Complex32 impedance)
        {
            if (IsNormalized == true)
                return (impedance / ReferenceImpedance.Impedance);
            return impedance;
        }

        public void DrawLegend()
        {
            //Plot.Annotations.Add(new TextAnnotation() { Text = "Blub", TextPosition = new DataPoint(0.2, -0.5) });
            OnSmithChartChanged();
        }

        public void Draw(SmithChartType type)
        {
            if (type == SmithChartType.Impedance && (IsImpedanceSmithChart == false))
            {
                IsImpedanceSmithChart = true;
            }
            else if (type == SmithChartType.Admittance && (IsAdmittanceSmithChart == false))
            {
                IsAdmittanceSmithChart = true;
            }
            else
                return;

            OnSmithChartChanged();
        }

        public void Clear(SmithChartType type)
        {
            if (type == SmithChartType.Impedance && IsImpedanceSmithChart)
            {
                IsImpedanceSmithChart = false;
            }
            else if (type == SmithChartType.Admittance && IsAdmittanceSmithChart)
            {
                IsAdmittanceSmithChart = false;
            }
            else
                return;

            OnSmithChartChanged();
        }

        private void CalculateMarker(Complex32 inputVal, SmithChartType type, bool refMarker)
        {
            Complex32 gamma = GetConformalGammaValue(inputVal, type, IsNormalized);
            DataPoint dataPoint = new DataPoint(gamma.Real, gamma.Imaginary);
            if (refMarker)
                RefMarkerSeries.Points.Add(dataPoint);
            else
                MarkerSeries.Points.Add(dataPoint);
        }

        private void CalculateIntermediateCurve(Complex32 inputValNew, Complex32 inputValOld, SmithChartType type, int numberOfPoints = 1000)
        {
            SCTLineSeries series = new SCTLineSeries { LineStyle = LineStyle.Solid, Color = OxyColor.FromRgb(50, 50, 50), StrokeThickness = 4 };

            Complex32 gamma;
            List <double> listReal= Lists.GetLinRange(inputValOld.Real, inputValNew.Real, numberOfPoints);
            List <double> listImaginary = Lists.GetLinRange(inputValOld.Imaginary, inputValNew.Imaginary, numberOfPoints);

            for ( int i = 0; i < numberOfPoints; i++)
            {
                gamma = GetConformalGammaValue(new Complex32((float)listReal[i], (float)listImaginary[i]), type, IsNormalized);
                series.Points.Add(new DataPoint(gamma.Real, gamma.Imaginary));
            }
            IntermediateCurveSeries.Add(series);
        }

        public void UpdateCurves(IList<InputImpedance> inputImpedances)
        {
            MarkerSeries.Points.Clear();
            RefMarkerSeries.Points.Clear();
            IntermediateCurveSeries.Clear();
            for (int i = 0; i<inputImpedances.Count-1; i++)
            {
                if (i > 0)
                {
                    CalculateIntermediateCurve(inputImpedances[i].Impedance, inputImpedances[i - 1].Impedance, inputImpedances[i].Type);
                }
                CalculateMarker(inputImpedances[i].Impedance, inputImpedances[i].Type, false);  
            }
            CalculateMarker(inputImpedances.Last().Impedance, inputImpedances.Last().Type, true);

            OnSmithChartCurvesChanged();
        }


        public SmithChart()
        {
            Frequency = 1.0e9;
            ReferenceImpedance = new ImpedanceElement(new Complex32(50, 0));
            IsNormalized = false;
            ConstRealImpedanceCircleSeries = new List<SCTLineSeries>();
            ConstImaginaryImpedanceCircleSeries = new List<SCTLineSeries>();
            ConstRealAdmittanceCircleSeries = new List<SCTLineSeries>();
            ConstImaginaryAdmittanceCircleSeries = new List<SCTLineSeries>();
            MarkerSeries = new SCTLineSeries();
            RefMarkerSeries = new SCTLineSeries();
            IntermediateCurveSeries = new List<SCTLineSeries>();
            IsImpedanceSmithChart = false;
            IsAdmittanceSmithChart = false;
            CalculateCircles(SmithChartType.Impedance);
            CalculateCircles(SmithChartType.Admittance);
        }

        public event EventHandler SmithChartChanged;
        protected void OnSmithChartChanged()
        {
            SmithChartChanged?.Invoke(this, new EventArgs());
        }

        public event EventHandler SmithChartCurvesChanged;
        protected void OnSmithChartCurvesChanged()
        {
            SmithChartCurvesChanged?.Invoke(this, new EventArgs());
        }
    }
}

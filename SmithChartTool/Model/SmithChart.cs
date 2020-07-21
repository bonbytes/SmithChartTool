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
        //public List<SCTLineSeries> ConstRealImpedanceCircleSeries { get; set; }
        //public List<SCTLineSeries> ConstImaginaryImpedanceCircleSeries { get; set; }
        //public List<SCTLineSeries> ConstRealAdmittanceCircleSeries { get; set; }
        //public List<SCTLineSeries> ConstImaginaryAdmittanceCircleSeries { get; set; }
        public List<List<Complex32>> ConstRealImpedanceCircles { get; set; }
        public List<List<Complex32>> ConstImaginaryImpedanceCircles { get; set; }
        public List<List<Complex32>> ConstRealAdmittanceCircles { get; set; }
        public List<List<Complex32>> ConstImaginaryAdmittanceCircles { get; set; }
        //public SCTLineSeries RefMarkerSeries { get; set; }
        //public SCTLineSeries MarkerSeries { get; set; }
        public List<Complex32> RefMarkers { get; set; }
        public List<Complex32> Markers { get; set; }
        //public List <SCTLineSeries> IntermediateCurveSeries { get; set; }
        public List<List<Complex32>> IntermediateCurves { get; set; }


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
            //List<SCTLineSeries> series = new List<SCTLineSeries>();
            List<Complex32> plotList = new List<Complex32>();
            List<double> reRangeFull = new List<double> { 0, 0.2, 0.5, 1, 2, 5, 10, 50};
            List<double> values = Lists.GetLogRange(Math.Log(1e-6, 10), Math.Log(1e6, 10), 500); // imaginary value of every circle
            var temp = values.Invert();
            var temp2 = values;
            temp2.Reverse();
            temp.AddRange(temp2);
            values = temp;
            //int i = 0;

            foreach (var re in reRangeFull) // for every real const circle
            {
                plotList.Clear();
                //series.Add(new SCTLineSeries { LineStyle = LineStyle.Solid, StrokeThickness = 0.75 });
                foreach (var im in values) // plot every single circle through conformal mapping
                {
                    Complex32 r = GetConformalGammaValue(new Complex32((float)re, (float)im), type, true);
                    //series[i].Points.Add(new DataPoint(r.Real, r.Imaginary));
                    plotList.Add(r);
                }
                if (type == SmithChartType.Impedance)
                    //ConstRealImpedanceCircleSeries.Add(series[i]);
                    ConstRealImpedanceCircles.Add(plotList);
                else if (type == SmithChartType.Admittance)
                    //ConstRealAdmittanceCircleSeries.Add(series[i]);
                    ConstRealAdmittanceCircles.Add(plotList);
                else
                    throw new ArgumentException("Wrong SmithChart Type", "type");
                //i++;
            }
        }

        private void CalculateConstImaginaryCircles(SmithChartType type)
        {
            //List<SCTLineSeries> series = new List<SCTLineSeries>();
            List<Complex32> plotList = new List<Complex32>();
            List<double> values = Lists.GetLogRange(Math.Log(1e-6, 10), Math.Log(1e6, 10), 1000); // real value of every circle
            List<double> imRange = new List<double>() { 0.2, 0.5, 1, 2, 5, 10, 20, 50 };
            List<double> imRangeFull = new List<double>(imRange.Invert());
            imRangeFull.Add(1e-20);  // "zero" line
            imRangeFull.AddRange(imRange);
            //int i = 0;
            foreach (var im in imRangeFull)
            {
                plotList.Clear();
                //series.Add(new SCTLineSeries { LineStyle = LineStyle.Dash, StrokeThickness = 0.75 });
                foreach (var re in values) // plot every single circle through conformal mapping
                {
                    Complex32 r = GetConformalGammaValue(new Complex32((float)re, (float)im), type, true);
                    //series[i].Points.Add(new DataPoint(r.Real, r.Imaginary));
                    plotList.Add(r);
                }
                if (type == SmithChartType.Impedance)
                    ConstImaginaryImpedanceCircles.Add(plotList);
                else if (type == SmithChartType.Admittance)
                    ConstImaginaryAdmittanceCircles.Add(plotList);
                else
                    throw new ArgumentException("Wrong SmithChart Type", "type");
                //i++;
            }
        }

        private void CalculateCircles(SmithChartType type)
        {
            CalculateConstRealCircles(type);
            CalculateConstImaginaryCircles(type);
        }

        public Complex32 GetConformalGammaValue(Complex32 input, SmithChartType inputType, bool isNormalized)
        {
            Complex32 _input = ImpedanceNormalized(input);
            
            if (inputType == SmithChartType.Impedance)
                return ((input - 1) / (input + 1));
            else if (inputType == SmithChartType.Admittance)
                return -((input - 1) / (input + 1));
            else
                return Complex32.NaN;
        }

        public Complex32 ImpedanceNormalized(Complex32 impedance)
        {
            if (IsNormalized == true)
                return (impedance / ReferenceImpedance.Impedance);
            return impedance;
        }

        private void DrawAxisText()
        {
            //Plot.Annotations.Add(new TextAnnotation() { Text = "Blub", TextPosition = new DataPoint(0.2, -0.5) });
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

            DrawAxisText();

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
            //DataPoint dataPoint = new DataPoint(gamma.Real, gamma.Imaginary);
            if (refMarker)
                RefMarkers.Add(gamma);
            //RefMarkerSeries.Points.Add(dataPoint);
            else
                Markers.Add(gamma);
                //MarkerSeries.Points.Add(dataPoint);
        }

        private void CalculateIntermediateCurve(Complex32 inputValNew, Complex32 inputValOld, SmithChartType type, int numberOfPoints = 1000)
        {
            //SCTLineSeries series = new SCTLineSeries { LineStyle = LineStyle.Solid, Color = OxyColor.FromRgb(50, 50, 50), StrokeThickness = 4 };
            List<Complex32> curve = new List<Complex32>();
 
            Complex32 gamma;
            List <double> listReal = Lists.GetLinRange(inputValOld.Real, inputValNew.Real, numberOfPoints);
            List <double> listImaginary = Lists.GetLinRange(inputValOld.Imaginary, inputValNew.Imaginary, numberOfPoints);

            for ( int i = 0; i < numberOfPoints; i++)
            {
                gamma = GetConformalGammaValue(new Complex32((float)listReal[i], (float)listImaginary[i]), type, IsNormalized);
                //series.Points.Add(new DataPoint(gamma.Real, gamma.Imaginary));
                curve.Add(gamma);
            }
            //IntermediateCurveSeries.Add(series);
            IntermediateCurves.Add(curve);
        }

        public void UpdateCurves(IList<InputImpedance> inputImpedances)
        {
            //MarkerSeries.Points.Clear();
            //RefMarkerSeries.Points.Clear();
            //IntermediateCurveSeries.Clear();
            Markers.Clear();
            RefMarkers.Clear();
            IntermediateCurves.Clear();
            
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
            //ConstRealImpedanceCircleSeries = new List<SCTLineSeries>();
            //ConstImaginaryImpedanceCircleSeries = new List<SCTLineSeries>();
            //ConstRealAdmittanceCircleSeries = new List<SCTLineSeries>();
            //ConstImaginaryAdmittanceCircleSeries = new List<SCTLineSeries>();
            //MarkerSeries = new SCTLineSeries();
            //RefMarkerSeries = new SCTLineSeries();
            //IntermediateCurveSeries = new List<SCTLineSeries>();
            ConstRealImpedanceCircles = new List<List<Complex32>>();
            ConstImaginaryImpedanceCircles = new List<List<Complex32>>();
            ConstRealAdmittanceCircles = new List<List<Complex32>>();
            ConstImaginaryAdmittanceCircles = new List<List<Complex32>>();
            Markers = new List<Complex32>();
            RefMarkers = new List<Complex32>();
            IntermediateCurves = new List<List<Complex32>>();
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

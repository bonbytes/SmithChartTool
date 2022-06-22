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

namespace SmithChartToolLibrary
{
    public enum SmithChartType
    {
        Impedance,
        Admittance
    }

    public enum MarkerType
    {
        Ref,
        Normal
    }

    public class SmithChart
    {
        public List<List<Complex32>> ImpedanceConstRealCircles { get; private set; }
        public List<List<Complex32>> ImpedanceConstImagCircles { get; private set; }
        public List<List<Complex32>> AdmittanceConstRealCircles { get; private set; }
        public List<List<Complex32>> AdmittanceConstImagCircles { get; private set; }
        public List<Complex32> RefMarkers { get; private set; }
        public List<Complex32> Markers { get; private set; }
        public List<List<Complex32>> IntermediateCurves { get; private set; }


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
                    OnSmithChartParametersChanged();
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
                    OnSmithChartChanged();
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
                    OnSmithChartParametersChanged();
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


        public Complex32 GetConformalGammaValue(Complex32 input)
        {
            Complex32 refvalue = ImpedanceNormalized(ReferenceImpedance.Impedance);
            input = ImpedanceNormalized(input);
            Complex32 calc = (input - refvalue) / (input + refvalue);
            return calc;
        }

        public Complex32 ImpedanceNormalized(Complex32 impedance)
        {
            if (IsNormalized == true)
                return (impedance / ReferenceImpedance.Impedance);
            return impedance;
        }

        private List<double> GenerateSCRealPlotRangeValues(double start = 0.0, double stop = 0.0)
        {
            List<double> vals = new List<double> { 
                0 * ReferenceImpedance.Impedance.Magnitude, 
                0.2 * ReferenceImpedance.Impedance.Magnitude, 
                0.5 * ReferenceImpedance.Impedance.Magnitude,
                1 * ReferenceImpedance.Impedance.Magnitude,
                2 * ReferenceImpedance.Impedance.Magnitude,
                20 * ReferenceImpedance.Impedance.Magnitude,
                100 * ReferenceImpedance.Impedance.Magnitude };

            return vals;
        }

        private List<double> GenerateSCImagPlotRangeValues(double start = 0.0, double stop = 0.0)
        {
            List<double> vals = new List<double> {
                0.2 * ReferenceImpedance.Impedance.Magnitude,
                0.5 * ReferenceImpedance.Impedance.Magnitude,
                1 * ReferenceImpedance.Impedance.Magnitude,
                2 * ReferenceImpedance.Impedance.Magnitude,
                20 * ReferenceImpedance.Impedance.Magnitude,
                100 * ReferenceImpedance.Impedance.Magnitude };

            return vals;
        }

        private List<List<Complex32>> CalculateConstRealCircles(SmithChartType type)
        {
            List<Complex32> plotList = new List<Complex32>();
            List<double> reRangeFull = GenerateSCRealPlotRangeValues();
            List<double> values = Lists.GetLogRange(Math.Log(1e-6, 10), Math.Log(1e6, 10), 500); // imaginary value of every const circle
            List<List<Complex32>> circles = new List<List<Complex32>>();
            var temp0 = new List<double>(values);
            temp0.Reverse();
            values = values.Invert();
            temp0.AddRange(values);
            values = temp0;

            foreach (var re in reRangeFull) // for every real const circle
            {
                plotList.Clear();
                foreach (var im in values) // plot every single circle through conformal mapping
                {
                    Complex32 input = new Complex32((float)re, (float)im); // impedance
                    if(type == SmithChartType.Admittance)
                    {
                        input = Complex32.Reciprocal(input);
                    }
                    Complex32 r = GetConformalGammaValue(input);
                    plotList.Add(r);
                }
                circles.Add(new List<Complex32>(plotList));
            }
            return circles;
        }

        private List<List<Complex32>> CalculateConstImagCircles(SmithChartType type)
        {
            List<Complex32> plotList = new List<Complex32>();
            List<double> values = Lists.GetLogRange(Math.Log(1e-6, 10), Math.Log(1e6, 10), 1000); // real values of every const (quarter-)circle
            List<List<Complex32>> circles = new List<List<Complex32>>();
            List<double> imRange = GenerateSCImagPlotRangeValues();
            List<double> imRangeFull = new List<double>(imRange.Invert());
            
            imRangeFull.Reverse();
            imRangeFull.Add(1e-20);  // "zero" horizontal line
            imRangeFull.AddRange(imRange);

            foreach (var im in imRangeFull) // for every imaginary const circle
            {
                plotList.Clear();
                foreach (var re in values) // plot every single circle through conformal mapping
                {
                    Complex32 input = new Complex32((float)re, (float)im); // impedance
                    if(type == SmithChartType.Admittance)
                    {
                        input = Complex32.Reciprocal(input);
                    }
                    Complex32 r = GetConformalGammaValue(input);
                    plotList.Add(r);
                }
                circles.Add(new List<Complex32>(plotList));
            }
            return circles;
        }

        public void Create(SmithChartType type)
        {
            Clear(type);
            CalculateConstRealCircles(type);
            CalculateConstImagCircles(type);
        }

        public void Clear(SmithChartType type)
        {
            if (type == SmithChartType.Impedance)
            {
                ImpedanceConstRealCircles.Clear();
                ImpedanceConstImagCircles.Clear();
            }
            else if (type == SmithChartType.Admittance)
            {
                AdmittanceConstRealCircles.Clear();
                AdmittanceConstImagCircles.Clear();
            }
            else
                throw new ArgumentException("Wrong SmithChart Type", "type");
        }

        private void CalculateMarker(Complex32 inputVal, SmithChartType type, MarkerType mtype)
        {
            Complex32 gamma = GetConformalGammaValue(inputVal);
            if (mtype == MarkerType.Ref)
                RefMarkers.Add(gamma);
            else if (mtype == MarkerType.Normal)
                Markers.Add(gamma);
        }

        private void CalculateIntermediateCurve(Complex32 inputValNew, Complex32 inputValOld, int numberOfPoints = 1000)
        {
            List<Complex32> plotPoints = new List<Complex32>();
 
            Complex32 gamma;
            List <double> listReal = Lists.GetLinRange(inputValOld.Real, inputValNew.Real, numberOfPoints);
            List <double> listImaginary = Lists.GetLinRange(inputValOld.Imaginary, inputValNew.Imaginary, numberOfPoints);

            for ( int i = 0; i < numberOfPoints; i++)
            {
                gamma = GetConformalGammaValue(new Complex32((float)listReal[i], (float)listImaginary[i]));
                plotPoints.Add(gamma);
            }
            IntermediateCurves.Add(plotPoints);
        }

        public void UpdateCurves(IList<InputImpedance> inputImpedances)
        {
            Markers.Clear();
            RefMarkers.Clear();
            IntermediateCurves.Clear();
            
            for (int i = 0; i<inputImpedances.Count-1; i++)
            {
                if (i > 0)
                {
                    CalculateIntermediateCurve(inputImpedances[i].Impedance, inputImpedances[i - 1].Impedance);
                }
                CalculateMarker(inputImpedances[i].Impedance, SmithChartType.Impedance, MarkerType.Normal);  
            }
            CalculateMarker(inputImpedances.Last().Impedance, SmithChartType.Impedance, MarkerType.Ref);
            OnSmithChartCurvesChanged();
        }


        public SmithChart()
        {
            Frequency = 1.0e9;
            ReferenceImpedance = new ImpedanceElement(new Complex32(50, 0));
            ImpedanceConstRealCircles = new List<List<Complex32>>();
            ImpedanceConstImagCircles = new List<List<Complex32>>();
            AdmittanceConstRealCircles = new List<List<Complex32>>();
            AdmittanceConstImagCircles = new List<List<Complex32>>();
            Markers = new List<Complex32>();
            RefMarkers = new List<Complex32>();
            IntermediateCurves = new List<List<Complex32>>();
            IsNormalized = true;
            IsImpedanceSmithChart = false;
            IsAdmittanceSmithChart = false;            
        }

        public event EventHandler SmithChartChanged;
        protected void OnSmithChartChanged()
        {
            SmithChartChanged?.Invoke(this, new EventArgs());
        }

        public event EventHandler SmithChartParametersChanged;
        protected void OnSmithChartParametersChanged()
        {
            SmithChartParametersChanged?.Invoke(this, new EventArgs());
        }

        public event EventHandler SmithChartCurvesChanged;
        protected void OnSmithChartCurvesChanged()
        {
            SmithChartCurvesChanged?.Invoke(this, new EventArgs());
        }
    }
}

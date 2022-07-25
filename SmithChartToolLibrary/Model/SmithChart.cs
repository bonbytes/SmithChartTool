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
using SmithChartToolLibrary.Utilities;

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

    public class
    {
        public List<List<Complex32>> ConstRealCircles { get; private set; }
        public List<List<Complex32>> ConstImagCircles { get; private set; }
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

        public int NumRealCircles { get; private set; }
        public int NumImagCircles { get; private set; }


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

        private int CalculateConstRealCircles()
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
                    plotList.Add(new Complex32((float)re, (float)im));
                }
                ConstRealCircles.Add(new List<Complex32>(plotList));
            }
            return 0;
        }

        private int CalculateConstImagCircles()
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
                    plotList.Add(new Complex32((float)re, (float)im));
                }
                ConstImagCircles.Add(new List<Complex32>(plotList));
            }
            return 0;
        }

        public void Create(SmithChartType type)
        {
            Clear();
            CalculateConstRealCircles();
            CalculateConstImagCircles();
        }

        public void Clear()
        {
            ConstRealCircles.Clear();
            ConstImagCircles.Clear();
        }

        private void CalculateMarker(Complex32 inputVal, SmithChartType type, MarkerType mtype)
        {
            Complex32 gamma = RF.GetConformalGammaValue(inputVal, this.ReferenceImpedance.Impedance);
            if (mtype == MarkerType.Ref)
                RefMarkers.Add(gamma);
            else if (mtype == MarkerType.Normal)
                Markers.Add(gamma);
        }

        private void CalculateIntermediateCurve(Complex32 inputValNew, Complex32 inputValOld, int numberOfPoints = 1000, SmithChartType type = SmithChartType.Impedance)
        {
            List<Complex32> plotPoints = new List<Complex32>();
 
            Complex32 gamma;
            List <double> listReal = Lists.GetLinRange(inputValOld.Real, inputValNew.Real, numberOfPoints);
            List <double> listImaginary = Lists.GetLinRange(inputValOld.Imaginary, inputValNew.Imaginary, numberOfPoints);

            for ( int i = 0; i < numberOfPoints; i++)
            {
                if(type == SmithChartType.Impedance)
                {
                    gamma = RF.GetConformalGammaValue(new Complex32((float)listReal[i], (float)listImaginary[i]), this.ReferenceImpedance.Impedance);
                    plotPoints.Add(gamma);
                }
                else if(type == SmithChartType.Admittance)
                {
                    gamma = RF.GetConformalGammaValue(new Complex32((float)listReal[i], (float)listImaginary[i]), this.ReferenceImpedance.Admittance);
                    plotPoints.Add(gamma);
                }  
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
                    CalculateIntermediateCurve(inputImpedances[i].Impedance, inputImpedances[i - 1].Impedance, 1000, inputImpedances[i].Type);
                    CalculateMarker(inputImpedances[i].Impedance, inputImpedances[i].Type, MarkerType.Normal);
                }
                  
            }
            CalculateMarker(inputImpedances.Last().Impedance, inputImpedances.Last().Type, MarkerType.Ref);
            OnSmithChartCurvesChanged();
        }


        public SmithChart()
        {
            Frequency = 1.0e9;
            ReferenceImpedance = new ImpedanceElement(new Complex32(50, 0));
            
            ConstRealCircles = new List<List<Complex32>>();
            ConstImagCircles = new List<List<Complex32>>();
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

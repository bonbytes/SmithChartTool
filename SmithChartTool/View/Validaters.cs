using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SmithChartTool.View
{
    public class ImpedanceRule : ValidationRule
    {
        public float RealMax { get; set; }
        public float RealMin { get; set; }
        public float ImaginaryMax { get; set; }
        public float ImaginaryMin { get; set; }

        public ImpedanceRule()
        {
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Complex32 cmplx = 0;

            try
            {
                if (((string)value).Length > 0)
                    cmplx = Complex32.Parse((string)value);
            }
            catch (Exception e)
            {
                return new ValidationResult(false, $"Illegal characters or {e.Message}");
            }

            if ((cmplx.Real < RealMin) || (cmplx.Real > RealMax) || (cmplx.Imaginary < ImaginaryMin) || (cmplx.Imaginary > ImaginaryMax))
            {
                return new ValidationResult(false, $"Please enter a complex value in the range: {RealMin},{ImaginaryMin} to {RealMax},{ImaginaryMax}.");
            }
            return ValidationResult.ValidResult;
        }
    }
}

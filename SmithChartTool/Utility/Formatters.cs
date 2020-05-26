using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmithChartTool.Utility
{
    public static class Formatters
    {
        public static string Complex32ToString(Complex32 val)
        {
            return string.Format("{0} {1} j{2}", val.Real, val.Imaginary >= 0 ? "+" : "-", Math.Abs(val.Imaginary));
        }
    }
}

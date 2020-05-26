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

        public static double StringPrefixToDouble(double num, string prefix)
        {
            switch (prefix)
            {
                case "f":
                    return num / Math.Pow(10, 12);
                case "p":
                    return num / Math.Pow(10, 9);
                case "µ":
                case "u":
                    return num / Math.Pow(10, 6);
                case "m":
                    return num / Math.Pow(10, 3);
                case "c":
                    return num / 100;
                case "d":
                    return num / 10;
                case "k":
                    return num * Math.Pow(10, 3);
                case "M":
                case "Meg":
                    return num * Math.Pow(10, 6);
                case "G":
                    return num * Math.Pow(10, 9);
                case "T":
                    return num * Math.Pow(10, 12);
                default:
                    return num;
            }
        }
    }
}

using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmithChartToolLibrary.Utilities
{
    public static class RF
    {
        public const double C0 = 299792458;  // speed of light in m/s
        public static double FrequencyToWavelength(double frequency)
        {
            return C0 / frequency;
        }

        public static double WavelengthToFrequency(double wavelength)
        {
            return C0 / wavelength;
        }

        public static double CalculatePropagationConstant(double wavelength)
        {
            return 2 * Math.PI / wavelength;
        }

        public static Complex32 CalculateSerialResistorResistance(double value)
        {
            return new Complex32((float)value, 0);
        }

        public static Complex32 CalculateParallelResistorConductance(double value)
        {
            return new Complex32((float)(1 / value), 0);
        }

        public static Complex32 CalculateSerialCapacitorReactance(double value, double frequency)
        {
            if (value == 0)
                return Complex32.Zero;
            return new Complex32(0, (float)(-1 / (2 * Math.PI * frequency * value)));
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
            if (value == 0)
                return Complex32.Zero;
            return new Complex32(0, (float)(1 / (2 * Math.PI * frequency * value)));
        }

        public static double CalculateVSWR(Complex32 gamma)
        {
            return (1 + gamma.Magnitude) / (1 - gamma.Magnitude);
        }
    }
}

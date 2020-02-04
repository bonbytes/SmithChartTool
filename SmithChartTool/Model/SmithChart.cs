using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;

namespace SmithChartTool.Model
{
    public class SmithChart
    {
        public double Frequency { get; private set; }
        public ImpedanceElement ReferenceImpedance { get; private set; }
        public bool IsNormalized { get; private set; }
        public Schematic Schematic { get; private set; }

        public SmithChart()
        {
            Frequency = 1.0e9;
            ReferenceImpedance = new ImpedanceElement(new Complex32(50, 0));
            IsNormalized = true;
            Schematic = new Schematic();
        }
    }
}

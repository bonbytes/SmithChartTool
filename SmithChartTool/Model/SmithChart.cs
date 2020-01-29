using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            this.Frequency = 1.0e9;
            this.ReferenceImpedance = new ImpedanceElement(new MathNet.Numerics.Complex32(50, 0));
            this.IsNormalized = true;
            this.Schematic = new Schematic();
        }
    }
}

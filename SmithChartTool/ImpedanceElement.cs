using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;


namespace SmithChartTool
{
    public class ImpedanceElement
    {
        private MathNet.Numerics.Complex32 _impedance;

        public ImpedanceElement()
        {
            this.Impedance = new MathNet.Numerics.Complex32(50, 0);
            this.isNormalized = true;
        }

        public ImpedanceElement(MathNet.Numerics.Complex32 impedance)
        {
            this.Impedance = _impedance;
            this.isNormalized = true;
        }

        public bool isNormalized { get; private set; }

        public MathNet.Numerics.Complex32 Impedance
        {
            get 
            {
                return this._impedance;
            }
            set
            {
                if(value.Real >= 0)
                {
                    this._impedance = value;
                }
                else
                    throw new Exception("Given impedance has negative real part.");
            }
        }



    }
}

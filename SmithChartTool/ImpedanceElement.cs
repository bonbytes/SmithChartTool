using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;


namespace SmithChartTool
{
    class ImpedanceElement
    {
        private MathNet.Numerics.Complex32 _impedance;
        private string _portName;
        private bool _normalized;

        public ImpedanceElement(MathNet.Numerics.Complex32 impedance)
        {
            this.Impedance = impedance;
        }

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

        public string PortName
        {
            get
            {
                return this._portName;
            }
            set
            {
                this._portName = value;
            }
        }

    }
}

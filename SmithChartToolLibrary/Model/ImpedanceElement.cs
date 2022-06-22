using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;


namespace SmithChartToolLibrary
{ 
    public class ImpedanceElement
    {
        private Complex32 _impedance;
        private Complex32 _admittance;
        public Complex32 Impedance
        {
            get 
            {
                return _impedance;
            }
            set
            {
                if(value != _impedance)
                {
                    if (Complex32.TryParse(value.ToString(), out Complex32 temp))
                    {
                        if (temp.Real >= 0)
                        {
                            _impedance = temp;
                            this.Admittance = Complex32.Reciprocal(temp);
                        }
                        else
                            throw new ArgumentException("Impedance has negative real part.", "Impedance");
                    }
                    else
                        throw new ArgumentException("Invalid Complex value representation", "Impedance");
                }  
            }
        }

        public Complex32 Admittance
        {
            get 
            {
                return (_admittance);
            }
            set
            {
                if(value != _admittance)
                {
                    if (Complex32.TryParse(value.ToString(), out Complex32 temp))
                    {
                        if (temp.Real >= 0)
                        {
                            _admittance = temp;
                            this.Impedance = Complex32.Reciprocal(temp);
                        }
                        else
                            throw new ArgumentException("Admittance has negative real part.", "Admittance");
                    }
                    else
                        throw new ArgumentException("Invalid Complex value representation", "Admittance");
                }  
            }
        }

        public ImpedanceElement()
        {
            Impedance = new Complex32(50,0);
        }

        public ImpedanceElement(Complex32 impedance)
        {
            Impedance = impedance;
        }
    }
}

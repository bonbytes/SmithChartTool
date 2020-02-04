using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;


namespace SmithChartTool.Model
{
    public class ImpedanceElement: INotifyPropertyChanged
    {
        private Complex32 _impedance;

        public ImpedanceElement()
        {
            Impedance = new Complex32(50, 0);
        }

        public ImpedanceElement(Complex32 impedance)
        {
            Impedance = impedance;
        }

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
                    if (value.Real >= 0) // value.IsRealNonNegative()
                    {
                        _impedance = value;
                        OnPropertyChanged("Impedance");
                    }
                    else
                        throw new Exception("Given impedance has negative real part.");
                }  
            }
        }

        #region INotifyPropertyChanged Members  
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
    }
}

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
        private MathNet.Numerics.Complex32 _impedance;

        public ImpedanceElement()
        {
            this.Impedance = new MathNet.Numerics.Complex32(50, 0);
        }

        public ImpedanceElement(MathNet.Numerics.Complex32 impedance)
        {
            this.Impedance = _impedance;
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
                    OnPropertyChanged("Impedance");
                }
                else
                    throw new Exception("Given impedance has negative real part.");
            }
        }

        #region INotifyPropertyChanged Members  
		
		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
    }
}

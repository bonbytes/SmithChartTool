using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmithChartTool.Model
{
    public class InputImpedance : ImpedanceElement
    {
		private int _Id;
		public int Id
		{
			get
			{
				return _Id;
			}
			set
			{
				_Id = value;
				OnPropertyChanged("UserId");
			}
		}

		public InputImpedance(int id, Complex32 impedance)
		{
			Id = id;
			Impedance = impedance;
		}


	}
}

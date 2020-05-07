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
		private int _id;
		public int Id
		{
			get
			{
				return _id;
			}
			set
			{
				if (value != _id)
				{
					if (!(int.TryParse(value.ToString(), out _id)))
						throw new ArgumentException("Invalid Id of input impedance", "Id");
				}
				
			}
		}

		public InputImpedance(int id, Complex32 impedance)
		{
			Id = id;
			Impedance = impedance;
		}


	}
}

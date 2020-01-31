using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmithChartTool
{
	public static class Extensions
	{
		public static List<double> Invert(this IEnumerable<double> list)
		{
			List<double> temp = new List<double>();
			foreach (var item in list)
			{
				temp.Add(item * -1);
			}
			return temp;
		}
	}
}

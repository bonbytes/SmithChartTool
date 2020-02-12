using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmithChartTool.Model;

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

		public static List<string> ToNames(this Type input)
		{
			List<string> temp = new List<string>();
			var a = Enum.GetValues(input);

			if(input.IsEnum)
			{
				foreach(Enum item in a)
				{
					if(a.ToString() != "Port")
						temp.Add(item.ToString());
				}
			}
			return temp;
		}
		public static Enum FromName(this Type input, string value)
		{
			if(value == "Port")
			{
				return null;
			}
			foreach(Enum item in Enum.GetValues(input))
			{
				if(item.ToString() == value)
				{
					return item;
				}
			}
			return null;
		}

	}
}

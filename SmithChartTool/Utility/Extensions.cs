using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmithChartTool.View;
using SmithChartTool.Model;
using System.Windows.Controls;
using System.Windows;

namespace SmithChartTool.Utility
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

			if (input.IsEnum)
			{
				foreach (Enum item in a)
				{
					var b = input.GetMember(item.ToString());
					if (b == null || b.Length < 1)
						continue;
				
					var c = b[0].GetCustomAttributes(typeof(HideInList), false);
					if (c == null || c.Length < 1)
						temp.Add(item.ToString());
				}

			}
			return temp;
		}
		public static Enum FromName(this Type input, string value)
		{
			foreach(Enum item in Enum.GetValues(input))
			{
				if(item.ToString() == value)
				{
					return item;
				}
			}
			return null;
		}

		//// Return the index of the item that is under the point in screen coordinates.
		//public static int IndexFromScreenPoint(this ListBox lst, Point point)
		//{
		//	// Convert the location to the ListBox's coordinates.
		//	point = lst.PointToScreen(point);
		//	// Return the index of the item at that position.
		//	return lst.IndexFromPoint(point);
		//}

	}
}

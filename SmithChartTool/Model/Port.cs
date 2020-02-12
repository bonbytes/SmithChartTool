using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;

namespace SmithChartTool.Model
{
	public class Port : SchematicElement
	{
		public string Name { get; private set; }
		public Port(int name, Complex32 impedance)
		{
			Name = name.ToString();
			Type = SchematicElementType.Port;
			Impedance = impedance;
		}
}
}

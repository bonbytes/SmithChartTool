using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmithChartTool.Model
{
	public class Port : SchematicElement
	{
		public string Name { get; private set; }

		public Port(int name, MathNet.Numerics.Complex32 impedance)
		{
			this.Type = SchematicElementType.Port;
			this.Id = name;
			this.Name = name.ToString();
			this.Impedance = impedance;
		}
}
}

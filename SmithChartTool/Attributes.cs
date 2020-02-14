using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmithChartTool
{
	public class SchematicElementInfo: Attribute
	{
		public string Name { get; }
		public string Icon { get; }
		public bool IsAddable { get; }

		public SchematicElementInfo(string name, string icon, bool isAddable)
		{
			this.Name = name;
			this.Icon = icon;
			this.IsAddable = isAddable;
		}

	}
}

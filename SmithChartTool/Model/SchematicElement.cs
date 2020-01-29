using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmithChartTool.Model
{
    public class SchematicElement : ImpedanceElement
    {
        public int Id { get; set; }
        public double Value { get; set; }
        public double Length { get; set; }
        public ESchematicElementType Type {get; set; }
    }
}

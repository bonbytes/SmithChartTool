using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmithChartTool.Model
{
    public enum ESchematicElementType
    {
        RESISTOR_SERIAL = 0,
        CAPACITOR_SERIAL,
        INDUCTOR_SERIAL,
        TLINE_SERIAL,
        RESISTOR_PARALLEL,
        CAPACITOR_PARALLEL,
        INDUCTOR_PARALLEL,
        TLINE_PARALLEL_OPEN,
        TLINE_PARALLEL,SHORT
    }

    public class Schematic
    {
        public Port P1 { get; private set; }
        public Port P2 { get; private set; }

        public IList<SchematicElement> SchematicElements { get; set; }

        public Schematic()
        {
            SchematicElements = new ObservableCollection<SchematicElement> { }; // create empty schematic
            this.P1 = new Port("P1");   // create port 1
            this.P2 = new Port("P2");   // create port 2
        }

        public void AddElement(ESchematicElementType eSchematicElement)
        {
            this.SchematicElements.Add(new SchematicElement 
            {
                    Id = this.SchematicElements.Count + 1, 
                    Type = eSchematicElement 
            });
        }
    }
}

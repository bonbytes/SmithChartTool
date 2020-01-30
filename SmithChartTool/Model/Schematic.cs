using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmithChartTool.Model
{
    public enum SchematicElementType
    {
        ResistorSerial = 0,
        CapacitorSerial,
        InductorSerial,
        ResistorParallel = 10,
        CapacitorParallel,
        InductorParallel,
        TransLineSerial = 20,
        TransLineParallelOpen,
        TransLineParallelShort
    }

    public class Schematic
    {
        public Port P1 { get; private set; }
        public Port P2 { get; private set; }

        public IList<SchematicElement> SchematicElements { get; private set; }

        public Schematic()
        {
            SchematicElements = new ObservableCollection<SchematicElement> { }; // create empty schematic
            this.P1 = new Port("P1");   // create port 1
            this.P2 = new Port("P2");   // create port 2
        }

        public void AddElement(SchematicElementType schematicElement)
        {
            this.SchematicElements.Add(new SchematicElement 
            {
                    Id = this.SchematicElements.Count + 1, 
                    Type = schematicElement 
            });
        }
        public void ChangeValue()
        {

        }
    }
}

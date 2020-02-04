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
        Port = 0,
        ResistorSerial = 1,
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
        public IList<SchematicElement> SchematicElements { get; private set; }

        public Schematic()
        {
            SchematicElements = new ObservableCollection<SchematicElement> { }; // create empty schematic
            SchematicElements.Add(new Port(1, new MathNet.Numerics.Complex32(50, 0)));
            SchematicElements.Add(new Port(2, new MathNet.Numerics.Complex32(50, 0)));
        }

        public void AddElement(SchematicElementType schematicElement)
        {
            this.SchematicElements.Add(new SchematicElement 
            {
                    Id = this.SchematicElements.Count + 1, 
                    Type = schematicElement 
            });
        }
        public void ChangePortImpedance(int name, MathNet.Numerics.Complex32 impedance)
        {
            if(name == 1)
            {
                this.SchematicElements.First().Impedance = impedance;
            }
            else
            {
                this.SchematicElements.Last().Impedance = impedance;
            }

        }

        public void ChangeElementValue(int index, double value)
        {
            this.SchematicElements[index + 1].Value = value;
        }
    }
}

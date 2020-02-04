using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;

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
            SchematicElements.Add(new Port(1, new Complex32(50, 0)));
            SchematicElements.Add(new Port(2, new Complex32(50, 0)));
        }

        public void AddElement(SchematicElementType schematicElement)
        {
            SchematicElements.Add(new SchematicElement 
            {
                    Id = SchematicElements.Count + 1, 
                    Type = schematicElement 
            });
        }
        public void ChangePortImpedance(int name, Complex32 impedance)
        {
            if(name == 1)
            {
                SchematicElements.First().Impedance = impedance;
            }
            else
            {
                SchematicElements.Last().Impedance = impedance;
            }

        }

        public void ChangeElementValue(int index, double value)
        {
            SchematicElements[index + 1].Value = value;
        }
    }
}

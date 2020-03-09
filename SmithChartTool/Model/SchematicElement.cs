using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmithChartTool.Model
{
    public enum SchematicElementType
    {   
        [SchematicElementInfo("Port", "Port","P"), HideInList] Port,
        [SchematicElementInfo("Resistor (serial)", "ResistorSerial", "R")] ResistorSerial,
        [SchematicElementInfo("Capacitor (serial)", "CapacitorSerial", "C")] CapacitorSerial,
        [SchematicElementInfo("Inductor (serial)", "InductorSerial", "L")] InductorSerial,
        [SchematicElementInfo("Resistor (parallel)", "ResistorParallel", "R")] ResistorParallel,
        [SchematicElementInfo("Capacitor (parallel)", "CapacitorParallel", "C")] CapacitorParallel,
        [SchematicElementInfo("Inductor (parallel)", "InductorParallel", "L")] InductorParallel,
        [SchematicElementInfo("Transmission Line", "TLine", "TL")] TLine,
        [SchematicElementInfo("Open stub", "OpenStub", "TL")] OpenStub,
        [SchematicElementInfo("Shorted stub", "ShortedStub", "TL")] ShortedStub,
        [SchematicElementInfo("Impedance (serial)", "ImpedanceSerial", "Z")] ImpedanceSerial,
        [SchematicElementInfo("Impedance (parallel)", "ImpedanceParallel", "Z")] ImpedanceParallel
    }

    public class SchematicElement : ImpedanceElement
    {
        private double _value;
        /// <summary>
        /// Value defines different mechanics based on element type
        /// Resistor: Value defines resistance
        /// Capacitor: Value defines capacitance
        /// Inductor: Value defines inductance
        /// Transmission Line: Value defines length
        /// </summary>
        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged("Value");
                }
            }
        }

        private int _designator;
        public int Designator
        {
            get
            {
                return _designator;
            }
            set
            {
                if (_designator != value)
                {
                    _designator = value;
                    OnPropertyChanged("Designator");
                }
            }
        }

        private SchematicElementType _type;
        public SchematicElementType Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged("Type");
                }
            }
        }

        public SchematicElement(SchematicElementType type, int designator, double value )
        {
            Type = type;
            Designator = designator;
            Value = value;

        }
        public SchematicElement()
        { }

        public string ToStringSimple()
        {
            return (Type.ToString() + " " + Designator.ToString() + " " + Value.ToString());
        }
    }
}

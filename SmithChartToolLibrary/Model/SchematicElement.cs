using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SmithChartToolLibrary
{
    public class SchematicElementInfo : Attribute
    {
        public string Name { get; }
        public string Icon { get; }
        public string Designator { get; }

        public SchematicElementInfo(string name, string icon, string designator)
        {
            Name = name;
            Icon = icon;
            Designator = designator;
        }
    }

    public class HideInList : Attribute
    {
        public HideInList()
        { }
    }

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
        /// <Value> defines different mechanics based on element type.
        /// Resistor: Value defines resistance in Ohms.
        /// Capacitor: Value defines capacitance in Farads.
        /// Inductor: Value defines inductance in Henries.
        /// Transmission Line: Value defines phase in degree.
        /// Open stub: Value defines phase in degree.
        /// Shorted stub: Value defines phase in degree.
        /// Generic Impedance (serial / parallel): No use.
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
                    if (!(double.TryParse(value.ToString(), out _value)))
                        throw new ArgumentException("Invalid value", "Value");
                    OnSchematicElementChanged("Value");
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
                    if (!(int.TryParse(value.ToString(), out _designator)))
                        throw new ArgumentException("Invalid designator", "Designator");
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
                    if (!(Enum.TryParse<SchematicElementType>(value.ToString(), out _type)))
                        throw new ArgumentException("Wrong SchematicElementType", "Type");
                }
            }
        }

        public ICommand RemoveSchematicElementCommand { get; set; }

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
         
        public event PropertyChangedEventHandler SchematicElementChanged;
        protected void OnSchematicElementChanged(string propertyName)
        {
            SchematicElementChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        
    }
}

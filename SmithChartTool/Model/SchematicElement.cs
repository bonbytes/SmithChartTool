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
        Port,
        ResistorSerial,
        CapacitorSerial,
        InductorSerial,
        ResistorParallel,
        CapacitorParallel,
        InductorParallel,
        TransLineSerial,
        TransLineParallelOpen,
        TransLineParallelShort
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
    }
}

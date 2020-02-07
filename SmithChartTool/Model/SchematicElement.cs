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
        private int _id;
        public int Id 
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        private double _value;
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmithChartTool.Model
{
    public class SchematicElement : ImpedanceElement
    {
        private int _id;
        public int Id 
        {
            get
            {
                return this._id;
            }
            set
            {
                if (this._id != value)
                {
                    this._id = value;
                    OnPropertyChanged("Id");
                }
            }
        }
        private double _value;
        public double Value
        {
            get
            {
                return this._value;
            }
            set
            {
                if (this._value != value)
                {
                    this._value = value;
                    OnPropertyChanged("Value");
                }
            }
        }
        
        private SchematicElementType _type;
        public SchematicElementType Type
        {
            get
            {
                return this._type;
            }
            set
            {
                if (this._type != value)
                {
                    this._type = value;
                    OnPropertyChanged("Type");
                }
            }
        }

        public SchematicElement()
        {

        }

    }
}

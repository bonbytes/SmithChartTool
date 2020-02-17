using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;

namespace SmithChartTool.Model
{
    public class Schematic: INotifyPropertyChanged
    {
        public List<string> AvailableElements { get; private set; }
        
        private ObservableCollection<SchematicElement> _elements;
        public ObservableCollection<SchematicElement> Elements
        {
            get { return _elements; }
            set 
            {
                _elements = value;
                OnPropertyChanged("Elements");
            }
        }
        public ObservableCollection<Complex32> _inputImpedances;
        public ObservableCollection<Complex32> InputImpedances
        {
            get { return _inputImpedances; }
            set 
            {
                _inputImpedances = value;
                OnPropertyChanged("InputImpedances");
            }
        }

        public Schematic()
        {
            Elements = new ObservableCollection<SchematicElement>();
            AvailableElements = typeof(SchematicElementType).ToNames();

            // create two Ports (initial setup)
            Elements.Add(new Port(1, new Complex32(50, 0)));
            Elements.Add(new Port(2, new Complex32(50, 0)));
        }

        public void AddElement(SchematicElementType schematicElement)
        {
            int index;
            if ((Elements.Count - 1) < 0)
                index = 0;
            else
                index = Elements.Count - 1;
                
            Elements.Insert(index, new SchematicElement 
            {
                    Type = schematicElement 
            });
        }
        public void InsertElement(int index, SchematicElementType schematicElement)
        {
            Elements.Insert(index, new SchematicElement
            {
                Type = schematicElement
            });
        }

        public void RemoveElement(int index)
        {
            Elements.RemoveAt(index);
        }

        public void ChangePortImpedance(int name, Complex32 impedance)
        {
            if(name == 1)
            {
                Elements[0].Impedance = impedance;
            }
            else
            {
                Elements[Elements.Count].Impedance = impedance;
            }
        }

        public void ChangeElementValue(int index, double value)
        {
            Elements[index].Value = value;
        }

        public void ChangeElementImpedance(int index, Complex32 impedance)
        {
            Elements[index].Impedance = impedance;
        }

        #region INotifyPropertyChanged Members  
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}

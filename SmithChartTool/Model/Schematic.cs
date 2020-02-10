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
        private ObservableCollection<SchematicElement> _schematicElements;

        public ObservableCollection<SchematicElement> SchematicElements
        {
            get { return _schematicElements; }
            set {
                OnPropertyChanged("SchematicElements");
                _schematicElements = value; }
        }

        public Schematic()
        {
            SchematicElements = new ObservableCollection<SchematicElement>(); // create empty schematic
            SchematicElements.Add(new Port(1, new Complex32(50, 0)));
            SchematicElements.Add(new Port(2, new Complex32(50, 0)));
        }

        public void AddElement(SchematicElementType schematicElement)
        {
            SchematicElements.Insert(SchematicElements.Count-1, new SchematicElement 
            {
                    Type = schematicElement 
            });
        }
        public void InsertElement(int index, SchematicElementType schematicElement)
        {
            SchematicElements.Insert(index, new SchematicElement
            {
                Type = schematicElement
            });
        }

        public void RemoveElement(int index)
        {
            SchematicElements.RemoveAt(index);

        }

        public void ChangePortImpedance(int name, Complex32 impedance)
        {
            if(name == 1)
            {
                SchematicElements[0].Impedance = impedance;
            }
            else
            {
                SchematicElements[SchematicElements.Count].Impedance = impedance;
            }

        }

        public void ChangeElementValue(int index, double value)
        {
            SchematicElements[index].Value = value;
        }

        public void ChangeElementImpedance(int index, Complex32 impedance)
        {
            SchematicElements[index].Impedance = impedance;
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using SmithChartTool.Utility;

namespace SmithChartTool.Model
{
    public class Schematic
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

        static int numResistors = 0;
        static int numCapacitors = 0;
        static int numInductors = 0;
        static int numTLines = 0;
        static int numImpedances = 0;

        public Schematic()
        {
            Elements = new ObservableCollection<SchematicElement>();
            AvailableElements = typeof(SchematicElementType).ToNames();

            // create two Ports (initial setup)
            Elements.Add(new SchematicElement() { Type = SchematicElementType.Port, Designator = 1, Impedance = new Complex32(50, 0), Value = 0 });
            Elements.Add(new SchematicElement() { Type = SchematicElementType.Port, Designator = 2, Impedance = new Complex32(50, 0), Value = 0 });
        }

        private void InvalidateDesignators()
        {
            int resistorDesignator = 1;
            int capacitorDesignator = 1;
            int inductorDesignator = 1;
            int tLineDesignator = 1;
            int impedanceDesignator = 1;
            
            foreach (var element in Elements)
            {
                if (element.Type == SchematicElementType.Port)
                    continue;
                switch (element.Type)
                {                    
                    case SchematicElementType.ResistorSerial:
                    case SchematicElementType.ResistorParallel:
                        element.Designator = resistorDesignator;
                        resistorDesignator++;
                        break;
                   
                    case SchematicElementType.CapacitorSerial:
                    case SchematicElementType.CapacitorParallel:
                        element.Designator = capacitorDesignator;
                        capacitorDesignator++;
                        break;
                    
                    case SchematicElementType.InductorSerial:
                    case SchematicElementType.InductorParallel:
                        element.Designator = inductorDesignator;
                        inductorDesignator++;
                        break;
                    
                    case SchematicElementType.TLine:
                    case SchematicElementType.OpenStub:
                    case SchematicElementType.ShortedStub:
                        element.Designator = tLineDesignator;
                        tLineDesignator++;
                        break;
                    
                    case SchematicElementType.ImpedanceSerial:
                    case SchematicElementType.ImpedanceParallel:
                        element.Designator = impedanceDesignator;
                        impedanceDesignator++;
                        break;
                    
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public void InsertElement(int index, SchematicElementType schematicElementType, double value = 0.0)
        {
            if ((Elements.Count - 1) < 0)
                index = 0;
            else if (index < 1)
            {
                index = Elements.Count - 1;
            }
            Elements.Insert(index, new SchematicElement
            {
                Type = schematicElementType,
                Value = value,
                Impedance = 0
            });
            IncreaseElementNumber(schematicElementType);
            InvalidateDesignators();
        }

        public void InsertElement(int index, SchematicElementType schematicElementType, Complex32 impedance, double value = 0.0)
        {
            if ((Elements.Count - 1) < 0)
                index = 0;
            else if (index < 1)
            {
                index = Elements.Count - 1;
            }
            Elements.Insert(index, new SchematicElement
            {
                Type = schematicElementType,
                Value = value,
                Impedance = impedance
            });
            IncreaseElementNumber(schematicElementType);
            InvalidateDesignators();
        }

        public void RemoveElement(int index)
        {
            DecreaseElementNumber(Elements[index].Type);
            Elements.RemoveAt(index);
            InvalidateDesignators();
        }

        private void IncreaseElementNumber(SchematicElementType schematicElementType)
        {
            switch (schematicElementType)
            {
                case SchematicElementType.ResistorSerial:
                case SchematicElementType.ResistorParallel:
                    numResistors++;
                    break;
                case SchematicElementType.CapacitorSerial:
                case SchematicElementType.CapacitorParallel:
                    numCapacitors++;
                    break;
                case SchematicElementType.InductorSerial:
                case SchematicElementType.InductorParallel:
                    numInductors++;
                    break;
                case SchematicElementType.TLine:
                case SchematicElementType.OpenStub:
                case SchematicElementType.ShortedStub:
                    numTLines++;
                    break;
                case SchematicElementType.ImpedanceSerial:
                case SchematicElementType.ImpedanceParallel:
                    numImpedances++;
                    break;

                default:
                    throw new NotImplementedException();
            }
        }
        private void DecreaseElementNumber(SchematicElementType schematicElementType)
        {
            switch (schematicElementType)
            {
                case SchematicElementType.ResistorSerial:
                case SchematicElementType.ResistorParallel:
                    numResistors--;
                    break;
                case SchematicElementType.CapacitorSerial:
                case SchematicElementType.CapacitorParallel:
                    numCapacitors--;
                    break;
                case SchematicElementType.InductorSerial:
                case SchematicElementType.InductorParallel:
                    numInductors--;
                    break;
                case SchematicElementType.TLine:
                case SchematicElementType.OpenStub:
                case SchematicElementType.ShortedStub:
                    numTLines--;
                    break;
                case SchematicElementType.ImpedanceSerial:
                case SchematicElementType.ImpedanceParallel:
                    numImpedances--;
                    break;

                default:
                    throw new NotImplementedException();
            }
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

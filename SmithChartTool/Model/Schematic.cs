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

        static int numResistors = 1;
        static int numCapacitors = 1;
        static int numInductors = 1;
        static int numTLines = 1;
        static int numImpedances = 1;

        public Schematic()
        {
            Elements = new ObservableCollection<SchematicElement>();
            AvailableElements = typeof(SchematicElementType).ToNames();

            // create two Ports (initial setup)
            Elements.Add(new SchematicElement() { Type = SchematicElementType.Port, Designator = 1, Impedance = new Complex32(50, 0), Value = 0 });
            Elements.Add(new SchematicElement() { Type = SchematicElementType.Port, Designator = 2, Impedance = new Complex32(50, 0), Value = 0 });
        }

        public void InvalidateDesignators()
        {
            // TODO: Re-Designate all elements when element is removed.
            foreach (var element in Elements)
            {
                switch (element.Type)
                {
                    case SchematicElementType.Port:
                        break;
                    
                    case SchematicElementType.ResistorSerial:
                    case SchematicElementType.ResistorParallel:
                        break;
                   
                    case SchematicElementType.CapacitorSerial:
                    case SchematicElementType.CapacitorParallel:
                        break;
                    
                    case SchematicElementType.InductorSerial:
                    case SchematicElementType.InductorParallel:
                        break;
                    
                    case SchematicElementType.TLine:
                    case SchematicElementType.OpenStub:
                    case SchematicElementType.ShortedStub:
                        break;
                    
                    case SchematicElementType.ImpedanceSerial:
                    case SchematicElementType.ImpedanceParallel:
                        break;
                    
                    default:
                        break;
                }
            }
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
            IncreaseElementDesignator(schematicElement);
        }

        public void InsertElement(int index, SchematicElementType schematicElement)
        {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 
            Elements.Insert(index, new SchematicElement
            {
                Type = schematicElement
            });
            IncreaseElementDesignator(schematicElement);
        }
        
        public void RemoveElement(int index)
        {
            Elements.RemoveAt(index);
            DecreaseElementDesignator(Elements[0].Type);
        }

        public void IncreaseElementDesignator(SchematicElementType type)
        {
            switch (type)
            {
                case SchematicElementType.ResistorSerial:
                    numResistors++;
                    break;
                case SchematicElementType.CapacitorSerial:
                    numCapacitors++;
                    break;
                case SchematicElementType.InductorSerial:
                    numInductors++;
                    break;
                case SchematicElementType.ResistorParallel:
                    numResistors++;
                    break;
                case SchematicElementType.CapacitorParallel:
                    numCapacitors++;
                    break;
                case SchematicElementType.InductorParallel:
                    numInductors++;
                    break;
                case SchematicElementType.TLine:
                    numTLines++;
                    break;
                case SchematicElementType.OpenStub:
                    numTLines++;
                    break;
                case SchematicElementType.ShortedStub:
                    numTLines++;
                    break;
                case SchematicElementType.ImpedanceSerial:
                    numImpedances++;
                    break;
                case SchematicElementType.ImpedanceParallel:
                    numImpedances++;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        public void DecreaseElementDesignator(SchematicElementType type)
        {
            switch (type)
            {
                case SchematicElementType.ResistorSerial:
                    numResistors--;
                    break;
                case SchematicElementType.CapacitorSerial:
                    numCapacitors--;
                    break;
                case SchematicElementType.InductorSerial:
                    numInductors--;
                    break;
                case SchematicElementType.ResistorParallel:
                    numResistors--;
                    break;
                case SchematicElementType.CapacitorParallel:
                    numCapacitors--;
                    break;
                case SchematicElementType.InductorParallel:
                    numInductors--;
                    break;
                case SchematicElementType.TLine:
                    numTLines--;
                    break;
                case SchematicElementType.OpenStub:
                    numTLines--;
                    break;
                case SchematicElementType.ShortedStub:
                    numTLines--;
                    break;
                case SchematicElementType.ImpedanceSerial:
                    numImpedances--;
                    break;
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

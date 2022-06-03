using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;

namespace SmithChartToolLibrary
{
    public class ObservableSchematicList : ObservableCollection<SchematicElement>
    {
        public ObservableSchematicList()
        {
            CollectionChanged += (s, e) =>
            {
                /* In case Move / Rearrange
                
                foreach (var item in e.OldItems)
                {
                    ((SchematicElement)item).SchematicElementChanged -= ChangeHandler;
                }
                foreach (var item in e.NewItems)
                {
                    ((SchematicElement)item).SchematicElementChanged += ChangeHandler;
                }

                */

                switch (e.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                        foreach (var item in e.NewItems)
                        {
                            ((SchematicElement)item).SchematicElementChanged += ChangeHandler;
                        }
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                        foreach (var item in e.OldItems)
                        {
                            ((SchematicElement)item).SchematicElementChanged -= ChangeHandler;
                        }
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                        break;
                    default:
                        break;
                }

            };

        }

        private void ChangeHandler(object sender, PropertyChangedEventArgs e)
        {
            OnSchematicElementChanged((SchematicElement)sender, e);
        }

        public event PropertyChangedEventHandler SchematicElementChanged;
        protected void OnSchematicElementChanged(SchematicElement item, PropertyChangedEventArgs e)
        {
            SchematicElementChanged?.Invoke(item, e);
        }

    }


    public class Schematic
    {
        public List<string> AvailableElements { get; set; }
        
        private ObservableSchematicList _elements;
        public ObservableSchematicList Elements
        {
            get { return _elements; }
            set 
            {
                if(value != _elements)
                {
                    _elements = value;
                    OnSchematicChanged();
                }
            }
        }

        static int numResistors = 0;
        static int numCapacitors = 0;
        static int numInductors = 0;
        static int numTLines = 0;
        static int numImpedances = 0;

        public Schematic()
        {
            //Elements = new ObservableCollection<SchematicElement>();
            Elements = new ObservableSchematicList();
            AvailableElements = typeof(SchematicElementType).ToNames();

            // create two Ports (initial setup)
            Elements.Add(new SchematicElement() { Type = SchematicElementType.Port, Designator = 1, Impedance = new Complex32(50, 0), Value = 0 });
            Elements.Add(new SchematicElement() { Type = SchematicElementType.Port, Designator = 2, Impedance = new Complex32(50, 0), Value = 0 });
        }

        private void UpdateDesignators()
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
                Impedance = new Complex32(0,0)
            });
            IncreaseElementNumber(schematicElementType);
            UpdateDesignators();
            OnSchematicChanged();
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
            UpdateDesignators();
            OnSchematicChanged();
        }

        public void RemoveElement(int index)
        {
            DecreaseElementNumber(Elements[index].Type);
            Elements.RemoveAt(index);
            UpdateDesignators();
            OnSchematicChanged();
        }

        public void Clear()
        {
            Elements.Clear();
            OnSchematicChanged();
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

        public event EventHandler SchematicChanged;
        protected void OnSchematicChanged()
        {
            SchematicChanged?.Invoke(this, new EventArgs());
        }
    }
}

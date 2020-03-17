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

        private double _frequency;
        public double Frequency
        {
            get
            {
                return _frequency;
            }
            set
            {
                if (value != _frequency)
                {
                    _frequency = value;
                    OnPropertyChanged("Frequency");
                }
            }
        }

        static int numResistors = 1;
        static int numCapacitors = 1;
        static int numInductors = 1;
        static int numTLines = 1;
        static int numImpedances = 1;
        public const double c0 = 299792458;  // speed of light in m/s

        public Schematic()
        {
            Frequency = 1.0e9;
            Elements = new ObservableCollection<SchematicElement>();
            AvailableElements = typeof(SchematicElementType).ToNames();
            InputImpedances = new ObservableCollection<Complex32>();

            // create two Ports (initial setup)
            Elements.Add(new SchematicElement() { Type = SchematicElementType.Port, Designator = 1, Impedance = new Complex32(50, 0), Value = 0 });
            Elements.Add(new SchematicElement() { Type = SchematicElementType.Port, Designator = 2, Impedance = new Complex32(50, 0), Value = 0 });

            InputImpedances.Add(Elements[0].Impedance);
            InputImpedances.Add(Elements[1].Impedance);
        }

        public static double FrequencyToWavelength(double frequency)
        {
            return c0 / frequency;
        }

        public static double WavelengthToFrequency(double wavelength)
        {
            return c0 / wavelength;
        }

        public static double CalculatePropagationConstant(double wavelength)
        {
            return (2 * Math.PI) / (wavelength);
        }


        public float CalculateSerialCapacitorReactance(double value, double frequency)
        {
            return (float)(1 / (2 * Math.PI * frequency * value));
        }

        public float CalculateSerialInductorReactance(double value, double frequency)
        {
            return (float)(2 * Math.PI * frequency * value);
        }

        public void InvalidateInputImpedances()
        {
            Complex32 transformationImpedance;

            for (int i = Elements.Count - 1; i>0; i--)
            {
                if (Elements[i].Type == SchematicElementType.Port)
                    InputImpedances[i] = Elements[i].Impedance;
                else
                { 
                    switch (Elements[i].Type)
                    {
                        case SchematicElementType.ResistorSerial:
                            transformationImpedance = new Complex32((float)Elements[i].Value, 0);
                            break;
                        case SchematicElementType.CapacitorSerial:
                            transformationImpedance = new Complex32(0, CalculateSerialCapacitorReactance(Elements[i].Value, Frequency));
                            break;
                        case SchematicElementType.InductorSerial:
                            transformationImpedance = new Complex32(0, CalculateSerialInductorReactance(Elements[i].Value, Frequency));
                            break;
                        case SchematicElementType.ResistorParallel:
                            transformationImpedance = 0;
                            break;
                        case SchematicElementType.CapacitorParallel:
                            transformationImpedance = 0;
                            break;
                        case SchematicElementType.InductorParallel:
                            transformationImpedance = 0;
                            break;
                        case SchematicElementType.TLine:
                            transformationImpedance = 0;
                            break;
                        case SchematicElementType.OpenStub:
                            transformationImpedance = 0;
                            break;
                        case SchematicElementType.ShortedStub:
                            transformationImpedance = 0;
                            break;
                        case SchematicElementType.ImpedanceSerial:
                            transformationImpedance = Elements[i].Impedance;
                            break;
                        case SchematicElementType.ImpedanceParallel:
                            transformationImpedance = 0;
                            break;
                        default:
                            transformationImpedance = 0;
                            throw new NotImplementedException("Invalid transformation. Aborting...");
                    }
                    InputImpedances[i] = InputImpedances[i + 1] + transformationImpedance;
                }
            }
            OnPropertyChanged("InputImpedances");
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
            InputImpedances.Add(Elements[index].Impedance);
            IncreaseElementDesignator(schematicElement);
            InvalidateInputImpedances();
        }

        public void InsertElement(int index, SchematicElementType schematicElement)
        {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 
            Elements.Insert(index, new SchematicElement
            {
                Type = schematicElement
            });
            InputImpedances.Add(Elements[index].Impedance);
            IncreaseElementDesignator(schematicElement);
            InvalidateInputImpedances();
        }
        
        public void RemoveElement(int index)
        {
            Elements.RemoveAt(index);
            InputImpedances.RemoveAt(index);
            DecreaseElementDesignator(Elements[0].Type);
            InvalidateInputImpedances();
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

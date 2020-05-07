using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmithChartTool.Model
{
    public class SCT
    {
        public SmithChart SC { get; private set; }
        public Schematic Schematic { get; private set; }
        public ObservableCollection<InputImpedance> _inputImpedances;
        public ObservableCollection<InputImpedance> InputImpedances
        {
            get { return _inputImpedances; }
            set
            {
                _inputImpedances = value;
                OnPropertyChanged("InputImpedances");
            }
        }
    }
}

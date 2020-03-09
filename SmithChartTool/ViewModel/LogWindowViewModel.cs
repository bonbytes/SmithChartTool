using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SmithChartTool.Model;
using SmithChartTool.View;

namespace SmithChartTool.ViewModel
{
    public class LogWindowViewModel: INotifyPropertyChanged
    {
        private LogWindow Window { get; set; }
        public Log LogData { get; private set; }
        private bool _isbtnResumeLogEnabled;
        public bool IsbtnResumeLogEnabled 
        { 
            get
            {
                return _isbtnResumeLogEnabled;
            }
            set
            {
                if(_isbtnResumeLogEnabled != value)
                {
                    _isbtnResumeLogEnabled = value;
                    OnPropertyChanged("IsbtnResumeLogEnabled");
                }
            }
        }
        private bool _isbtnCloseLogEnabled;
        public bool IsbtnCloseLogEnabled
        {
            get
            {
                return _isbtnCloseLogEnabled;
            }
            set
            {
                if (_isbtnCloseLogEnabled != value)
                {
                    _isbtnCloseLogEnabled = value;
                    OnPropertyChanged("IsbtnCloseLogEnabled");
                }
            }
        }
        private bool _isbtnStopLogEnabled;
        public bool IsbtnStopLogEnabled
        {
            get
            {
                return _isbtnStopLogEnabled;
            }
            set
            {
                if (_isbtnStopLogEnabled != value)
                {
                    _isbtnStopLogEnabled = value;
                    OnPropertyChanged("IsbtnStopLogEnabled");
                }
            }
        }

        public static RoutedUICommand CommandCloseLog = new RoutedUICommand("Close Log", "CL", typeof(LogWindow));
        public static RoutedUICommand CommandStopLog = new RoutedUICommand("Stop Log", "SL", typeof(LogWindow));
        public static RoutedUICommand CommandResumeLog = new RoutedUICommand("Resume Log", "RL", typeof(LogWindow));


        public LogWindowViewModel(Log logData)
        {
            LogData = logData;
            IsbtnResumeLogEnabled = false;
            IsbtnCloseLogEnabled = true;
            IsbtnStopLogEnabled = true;

            Window = new LogWindow(this);

            Window.CommandBindings.Add(new CommandBinding(CommandCloseLog, (s, e) => { RunCloseLog(); }));
            Window.CommandBindings.Add(new CommandBinding(CommandStopLog, (s, e) => { RunStopLog(); }));
            Window.CommandBindings.Add(new CommandBinding(CommandResumeLog, (s, e) => { RunResumeLog(); }));

            Window.Show();
        }

        private void RunCloseLog()
        {
            Window.Close();
        }

        private void RunStopLog()
        {
            LogData.AddLine("[log] ### Logging stopped. ###\r");

            IsbtnStopLogEnabled = false;
            IsbtnResumeLogEnabled = true;
        }

        private void RunResumeLog()
        {
            LogData.Lines.Clear();

            LogData.AddLine("[log] ### Resuming log... ###\r");

            IsbtnStopLogEnabled = true;
            IsbtnResumeLogEnabled = false;
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

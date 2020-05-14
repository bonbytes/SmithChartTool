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
    public class LogViewModel: INotifyPropertyChanged
    {
        private LogWindow Window { get; set; }
        public Log LogData { get; private set; }

        public static RoutedUICommand CommandClose = new RoutedUICommand("Close", "Close", typeof(LogWindow));

        public LogViewModel(Log logData)
        {
            LogData = logData;
            Window = new LogWindow(this);

            Window.CommandBindings.Add(new CommandBinding(CommandClose, (s, e) => { RunClose(); }));
            
            Window.Show();
        }

        private void RunClose()
        {
            Window.Close();
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

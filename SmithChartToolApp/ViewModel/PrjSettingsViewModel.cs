using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SmithChartToolApp.View;
using SmithChartToolLibrary;


namespace SmithChartToolApp.ViewModel
{
    public class PrjSettingsViewModel: INotifyPropertyChanged
    {
        public Project ProjectData { get; private set; }
        private PrjSettingsWindow Window { get; set; }

        public static RoutedUICommand CommandClose = new RoutedUICommand("Close", "Close", typeof(PrjSettingsWindow));

        public PrjSettingsViewModel(Project projectData)
        {
            ProjectData = projectData;
            Window = new PrjSettingsWindow(this);
            Window.CommandBindings.Add(new CommandBinding(CommandClose, (s, e) => { RunClose(); }));

            Window.Show();
        }

        void RunClose()
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

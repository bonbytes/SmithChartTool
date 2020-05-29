using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SmithChartTool.View;


namespace SmithChartTool.ViewModel
{
    public class SettingsViewModel: INotifyPropertyChanged
    {
        private SettingsWindow Window { get; set; }

        public static RoutedUICommand CommandClose = new RoutedUICommand("Close", "Close", typeof(SettingsWindow));

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsViewModel()
        {
            Window = new SettingsWindow(this);
            Window.CommandBindings.Add(new CommandBinding(CommandClose, (s, e) => { RunClose(); }));

            Window.Show();
        }

        void RunClose()
        {
            Window.Close();
        }

    }
}

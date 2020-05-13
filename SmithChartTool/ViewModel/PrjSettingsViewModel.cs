using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SmithChartTool.Model;
using SmithChartTool.View;


namespace SmithChartTool.ViewModel
{
    public class PrjSettingsViewModel
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

    }
}

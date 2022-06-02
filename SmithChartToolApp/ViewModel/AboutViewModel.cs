using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SmithChartToolApp.View;


namespace SmithChartToolApp.ViewModel
{
    public class AboutViewModel
    {
        private AboutWindow Window { get; set; }

        public static RoutedUICommand CommandClose = new RoutedUICommand("Close", "Close", typeof(AboutWindow));

        public AboutViewModel()
        {
            Window = new AboutWindow(this);
            Window.CommandBindings.Add(new CommandBinding(CommandClose, (s, e) => { RunClose(); }));

            Window.Show();
        }

        void RunClose()
        {
            Window.Close();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SmithChartTool.View;


namespace SmithChartTool.ViewModel
{
    public class AboutWindowViewModel
    {
        private AboutWindow Window { get; set; }

        public static RoutedUICommand CommandOK = new RoutedUICommand("OK", "OK", typeof(AboutWindow));

        public AboutWindowViewModel()
        {
            Window = new AboutWindow(this);
            Window.CommandBindings.Add(new CommandBinding(CommandOK, (s, e) => { RunOK(); }));

            Window.Show();
        }

        void RunOK()
        {
            Window.Close();
        }

    }
}

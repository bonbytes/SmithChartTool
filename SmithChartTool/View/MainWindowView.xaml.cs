using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SmithChartTool.ViewModel;
using SmithChartTool.Model;

namespace SmithChartTool.View
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        public MainWindowViewModel VM { get; set; } = new MainWindowViewModel();

        public MainWindowView()
        {

            this.InitializeComponent();
            this.DataContext = VM;
            CommandBindings.Add(new CommandBinding(MainWindowViewModel.CommandXYAsync, (s, e) => { VM.RunCommandXYAsync(); }, (s, e) => { Debug.Print("Blub"); })); //e.CanExecute = bli; }));
            
        }
    }
}

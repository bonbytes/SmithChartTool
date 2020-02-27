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
using System.Windows.Markup;
using OxyPlot;

namespace SmithChartTool.View
{
    public partial class MainWindow : Window
    {
        public MainWindowViewModel VM { get; set; } = new MainWindowViewModel();
        

        public MainWindow()
        {
            this.DataContext = VM;
            this.InitializeComponent();

           // CommandBindings.Add(new CommandBinding(MainWindowViewModel.CommandTestFeature, (s, e) => { VM.RunTestFeature(); }));
            //CommandBindings.Add(new CommandBinding(MainWindowViewModel.CommandXYAsync, (s, e) => { VM.RunXYAsync(); }, (s, e) => { Debug.Print("Blab"); })); //e.CanExecute = bli; }));
            //CommandBindings.Add(new CommandBinding(MainWindowViewModel.CommandSaveSmithChartImage, (s, e) => { VM.RunSaveSmithChartImage(); }));
            

            this.Loaded += (s, e) => {
                //mitn doofen Frame -> cool, weil Datei (muss als Page gepackt sein)
                //jaggeline.Navigate(new Uri("pack://application:,,,/Images/SchematicElements/Zeichnung.xaml"));

                //Resource finden, doof weil in ResourceDictionary verbaut
                //var a = FindResource("ResistorSerial");
                //herbert.Content = a;

                //DynamicResource attachen, doof weil s.o.
                //herbert.SetResourceReference(Button.ContentProperty, "ResistorSerial");

                //xaml laden und verarbeiten, cool weil Datei (muss als Resource gepackt sein)
                //var sri = Application.GetResourceStream(new Uri("pack://application:,,,/Images/SchematicElements/Zeichnung.xaml"));
                //var a = XamlReader.Load(sri.Stream);
                //herbert.Content = a;


                this.oxySmithChart.ActualController.UnbindMouseDown(OxyMouseButton.Left);
                this.oxySmithChart.ActualController.BindMouseEnter(PlotCommands.HoverPointsOnlyTrack);

                List<string> themes = new List<string>();
                themes.Add("LightTheme");
                themes.Add("DarkTheme");
                cmbThemes.DataContext = themes;

                cmbThemes.SelectionChanged += (_s, _e) =>
                {
                    Application.Current.Resources.MergedDictionaries.Clear();

                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/Themes/" + cmbThemes.SelectedItem + ".xaml") });

                };
            };
        }
    }
}

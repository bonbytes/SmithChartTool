using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using SmithChartTool.View;
using SmithChartTool.Model;
using SmithChartTool.Utility;
using MathNet.Numerics;
using System.IO;
using OxyPlot;
using OxyPlot.Series;
using System.Windows.Controls;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Media;

namespace SmithChartTool.ViewModel
{
    public class MainViewModel : IDragDrop, INotifyPropertyChanged
    {
        private MainWindow Window { get; set; }
        public SCT Model { get; set; }

        public string ProjectName
        {
            get
            {
                return Model.ProjectName;
            }
            set
            {
                if (Model.ProjectName != value)
                {
                    Model.ProjectName = value;
                    OnPropertyChanged("ProjectName");
                }

            }
        }
        public string ProjectPath
        {
            get
            {
                return Model.ProjectPath;
            }
            set
            {
                if (Model.ProjectPath != value)
                {
                    Model.ProjectPath = value;
                    OnPropertyChanged("ProjectPath");
                }

            }
        }
        public string ProjectDescription
        {
            get
            {
                return Model.ProjectDescription;
            }
            set
            {
                if (Model.ProjectDescription != value)
                {
                    Model.ProjectDescription = value;
                    OnPropertyChanged("ProjectDescription");
                }

            }
        }
        public double Frequency
        {
            get
            {
                return Model.SC.Frequency;
            }
            set
            {
                if (Model.SC.Frequency != value)
                {
                    Model.SC.Frequency = value;
                    Model.InvalidateInputImpedances();
                    OnPropertyChanged("Frequency");
                }

            }
        }
        //public Complex32 ReferenceImpedance
        //{
        //    get
        //    {
        //        return Model.SC.ReferenceImpedance.Impedance;
        //    }
        //    set
        //    {
        //        if (value != Model.SC.ReferenceImpedance.Impedance)
        //        {
        //            Model.SC.ReferenceImpedance.Impedance = value;
        //            Model.InvalidateInputImpedances();
        //            OnPropertyChanged("ReferenceImpedance");
        //        }
        //    }
        //}
        public ImpedanceElement ReferenceImpedance
        {
            get
            {
                return Model.SC.ReferenceImpedance;
            }
            set
            {
                if (value != Model.SC.ReferenceImpedance)
                {
                    Model.SC.ReferenceImpedance = value;
                    Model.InvalidateInputImpedances();
                    OnPropertyChanged("ReferenceImpedance");
                }
            }
        }
        public bool IsNormalized
        {
            get
            {
                return Model.SC.IsNormalized;
            }
            set
            {
                if (value != Model.SC.IsNormalized)
                {
                    Model.SC.IsNormalized = value;
                    Model.InvalidateInputImpedances();
                    OnPropertyChanged("IsNormalized");
                }
            }
        }
        public ObservableCollection<SchematicElement> SchematicElements
        {
            get
            {
                return Model.Schematic.Elements;
            }
            set
            {
                if(value != Model.Schematic.Elements)
                {
                    Model.Schematic.Elements = value;
                    Model.InvalidateInputImpedances();
                    OnPropertyChanged("SchematicElements");
                }
            }
        }

        public ObservableCollection<InputImpedance> InputImpedances
        {
            get
            {
                return Model.InputImpedances;
            }
            set
            {
                if (value != Model.InputImpedances)
                {
                    Model.InputImpedances = value;
                    OnPropertyChanged("InputImpedances");
                }
            }
        }
        private bool _isImpedanceSmithChartShown;
        public bool IsImpedanceSmithChartShown
        {
            get
            {
                return _isImpedanceSmithChartShown;
            }
            set
            {
                if (value != _isImpedanceSmithChartShown)
                {
                    _isImpedanceSmithChartShown = value;
                    OnPropertyChanged("IsImpedanceSmithChartShown");
                }
            }
        }
        private bool _isAdmittanceSmithChartShown;
        public bool IsAdmittanceSmithChartShown
        {
            get
            {
                return _isAdmittanceSmithChartShown;
            }
            set
            {
                if (value != _isAdmittanceSmithChartShown)
                {
                    _isAdmittanceSmithChartShown = value;
                    OnPropertyChanged("IsAdmittanceSmithChartShown");
                }
            }
        }

        public static RoutedUICommand CommandTestFeature = new RoutedUICommand("Run Test Feature", "RTFE", typeof(MainWindow), new InputGestureCollection() { new KeyGesture(Key.T, ModifierKeys.Control) });
        public static RoutedUICommand CommandViewHelp = new RoutedUICommand("View Help", "VH", typeof(MainWindow));
        public static RoutedUICommand CommandShowLogWindow = new RoutedUICommand("Show Log Window", "SLW", typeof(MainWindow), new InputGestureCollection() { new KeyGesture(Key.L, ModifierKeys.Control) });
        public static RoutedUICommand CommandShowAboutWindow = new RoutedUICommand("Show About Window", "SAW", typeof(MainWindow));
        public static RoutedUICommand CommandNewProject = new RoutedUICommand("New project file", "PN", typeof(MainWindow), new InputGestureCollection() { new KeyGesture(Key.N, ModifierKeys.Control) });
        public static RoutedUICommand CommandSaveProject = new RoutedUICommand("Save project file", "PS", typeof(MainWindow), new InputGestureCollection() { new KeyGesture(Key.S, ModifierKeys.Control) });
        public static RoutedUICommand CommandSaveProjectAs = new RoutedUICommand("Save project file as", "PSA", typeof(MainWindow));
        public static RoutedUICommand CommandOpenProject = new RoutedUICommand("Open project file", "PO", typeof(MainWindow), new InputGestureCollection() { new KeyGesture(Key.O, ModifierKeys.Control) });
        public static RoutedUICommand CommandExportSmithChartImage = new RoutedUICommand("Export Smith Chart image", "ESCI", typeof(MainWindow), new InputGestureCollection() { new KeyGesture(Key.P, ModifierKeys.Control) });
        public static RoutedUICommand CommandCloseApp = new RoutedUICommand("CloseApplication", "EXIT", typeof(MainWindow), new InputGestureCollection() { new KeyGesture(Key.F4, ModifierKeys.Alt) });
        public static RoutedUICommand CommandXYAsync = new RoutedUICommand("Run XY Async", "RXYA", typeof(MainWindow), new InputGestureCollection() { new KeyGesture(Key.F5), new KeyGesture(Key.F5, ModifierKeys.Control) });

        public MainViewModel()
        {
            Model = new SCT();
            Window = new MainWindow(this);
            Window.CommandBindings.Add(new CommandBinding(CommandTestFeature, (s, e) => { RunTestFeature(); }));
            Window.CommandBindings.Add(new CommandBinding(CommandXYAsync, (s, e) => { RunXYAsync(); }, (s, e) => { Debug.Print("Blab"); })); //e.CanExecute = bli; }));
            Window.CommandBindings.Add(new CommandBinding(CommandExportSmithChartImage, (s, e) => { RunExportSmithChartImage(); }));
            Window.CommandBindings.Add(new CommandBinding(CommandShowLogWindow, (s, e) => { RunShowLogWindow(); }));
            Window.CommandBindings.Add(new CommandBinding(CommandViewHelp, (s, e) => { RunViewHelp(); }));
            Window.CommandBindings.Add(new CommandBinding(CommandShowAboutWindow, (s, e) => { RunShowAboutWindow(); }));
            Window.CommandBindings.Add(new CommandBinding(CommandNewProject, (s, e) => { RunNewProject(); }));
            Window.CommandBindings.Add(new CommandBinding(CommandSaveProject, (s, e) => { RunSaveProject(); }));
            Window.CommandBindings.Add(new CommandBinding(CommandSaveProjectAs, (s, e) => { RunSaveProjectAs(); }));
            Window.CommandBindings.Add(new CommandBinding(CommandOpenProject, (s, e) => { RunOpenProject(); }));
            Window.CommandBindings.Add(new CommandBinding(CommandCloseApp, (s, e) => { RunCloseApp(); }));
            Window.oxySmithChart.ActualController.UnbindMouseDown(OxyMouseButton.Left);
            Window.oxySmithChart.ActualController.BindMouseEnter(OxyPlot.PlotCommands.HoverPointsOnlyTrack);

            Window.Show();
        }

        public void DropSchematicElement(int index, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("SchematicElement"))
            {
                SchematicElementType type = (SchematicElementType)e.Data.GetData("SchematicElement");
                Model.InsertSchematicElement(index, type);
            }
        }

        public async void RunTestFeature()
        {
            await Task.Run(() => MessageBox.Show(Model.SC.ReferenceImpedance.Impedance.ToString()));
        }

        public void RunNewProject()
        {
            Model.NewProject();
        }

        public void RunSaveProjectAs()
        {
            SaveFileDialog fd = new SaveFileDialog();
 
            fd.Title = "Save project file...";
            fd.Filter = "SCTPRJ|*.sctprj";
            fd.ShowDialog();

            if (fd.FileName != string.Empty)
            {
                string fileExt = Path.GetExtension(fd.FileName);
                Model.SaveProjectAs(fd.FileName, fileExt);
                ProjectPath = fd.FileName;
            }
        }

        public void RunSaveProject()
        {
            if (ProjectPath != string.Empty)
            {
                Model.SaveProjectAs(ProjectPath);
            }
            else
            {
                RunSaveProjectAs();
            }
        }

        public void RunOpenProject()
        {
            OpenFileDialog fd = new OpenFileDialog();

            fd.Filter = "SCTPRJ|*.sctprj";
            fd.Title = "Open project file...";

            fd.ShowDialog();

            if (fd.FileName != string.Empty)
            {
                string fileExt = Path.GetExtension(fd.FileName);
                Model.OpenProject(fd.FileName, fileExt);
                ProjectPath = fd.FileName;
            }
        }

        public void RunExportSmithChartImage()
        {
            SaveFileDialog fd = new SaveFileDialog();

            fd.Filter = "PNG|*.png|BMP|*.bmp|JPEG|*.jpeg,*.jpg";
            fd.Title = "Export Smith Chart image...";

            fd.ShowDialog();

            if (fd.FileName != string.Empty)
            {
                string ImExt = Path.GetExtension(fd.FileName);
                Model.ExportSmithChart(fd.FileName);
            }
        }

        public void RunViewHelp()
        {
            MessageBox.Show("Setup project Name and Description \r\n Setup Smith-Chart frequency and reference impedance in <Smith-Chart settings> \r\n Drag and drop Schematic Elements from <Element selection> to <Schematic> \r\n Change values of schematic elements via embedded TextBox \r\n To delete elements, press red X while hovering respective element");
        }

        public void RunShowLogWindow()
        {
            var logWindowViewModel = new LogViewModel(Model.LogData);
        }

        public void RunShowAboutWindow()
        {
            var aboutWindowViewModel = new AboutViewModel();
        }

        public void RunCloseApp()
        {
            Application.Current.Shutdown();
        }

        public async void RunXYAsync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            await XYAsync();
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            MessageBox.Show(elapsedMs.ToString());
        }

        private async Task XYAsync()
        {
            var result = await Task.Run(() => 0);  // insert lambda body

            /// or (in case of parallel run)
            //List<Task> tasks = new List<Task>();
            //foreach (string data in websites)
            //{
            //    tasks.Add(RunXYAsync(data));
            //}
            //var results = await Task.WhenAll(tasks);
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

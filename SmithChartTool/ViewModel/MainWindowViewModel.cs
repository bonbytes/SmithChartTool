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
    public class MainWindowViewModel : IDragDrop, INotifyPropertyChanged
    {
        public enum StatusType
        {
            Ready,
            Busy,
            Error
        }

        private MainWindow Window { get; set; }
        public SmithChart SC { get; private set; }
        public Schematic Schematic { get; private set; }

        public string ProjectName { get; private set; }
        public string ProjectPath { get; private set; }
        public string ProjectDescription { get; private set; }
        public int Progress { get; private set; }

        public Log LogData { get; set; }

        public static event Action<int> ProgressChanged;
        public static event Action<StatusType> StatusChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public static RoutedUICommand CommandTestFeature = new RoutedUICommand("Run Test Feature", "RTFE", typeof(MainWindow));
        public static RoutedUICommand CommandShowLogWindow = new RoutedUICommand("Show Log Window", "SLW", typeof(MainWindow));
        public static RoutedUICommand CommandShowAboutWindow = new RoutedUICommand("Show About Window", "SAW", typeof(MainWindow));
        public static RoutedUICommand CommandSaveSmithChartImage = new RoutedUICommand("Save Smith Chart image", "RSSCI", typeof(MainWindow));
        public static RoutedUICommand CommandXYAsync = new RoutedUICommand("Run XY Async", "RXYA", typeof(MainWindow), new InputGestureCollection() { new KeyGesture(Key.F5), new KeyGesture(Key.R, ModifierKeys.Control) });

        private const int ProgressUpdateIntervall = 400;
        private const int FinishedDelay = 400;
        private const char HeaderMarker = '#';
        private const char DataMarker = '!';

        public MainWindowViewModel()
        {
            LogData = new Log();
            SC = new SmithChart();
            Schematic = new Schematic();
            AddSchematicElement(SchematicElementType.ImpedanceSerial);
            
            Window = new MainWindow(this);
            Window.CommandBindings.Add(new CommandBinding(CommandTestFeature, (s, e) => { RunTestFeature(); }));
            Window.CommandBindings.Add(new CommandBinding(CommandXYAsync, (s, e) => { RunXYAsync(); }, (s, e) => { Debug.Print("Blab"); })); //e.CanExecute = bli; }));
            Window.CommandBindings.Add(new CommandBinding(CommandSaveSmithChartImage, (s, e) => { RunSaveSmithChartImage(); }));
            Window.CommandBindings.Add(new CommandBinding(CommandShowLogWindow, (s, e) => { RunShowLogWindow(); }));
            Window.CommandBindings.Add(new CommandBinding(CommandShowAboutWindow, (s, e) => { RunShowAboutWindow(); }));
            Window.oxySmithChart.ActualController.UnbindMouseDown(OxyMouseButton.Left);
            Window.oxySmithChart.ActualController.BindMouseEnter(OxyPlot.PlotCommands.HoverPointsOnlyTrack);

            Window.Show();
        }

        public void DropSchematicElement(int index, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("SchematicElement"))
            {
                SchematicElementType type = (SchematicElementType)e.Data.GetData("SchematicElement");
                Schematic.InsertElement(index, type);
            }
        }

        public void AddSchematicElement(SchematicElementType type)
        {
            string typeDescription = "";
            Schematic.AddElement(type);
            
            Type t = type.GetType();
            var b = t.GetMember(type.ToString());

            if (b.Count() > 0)
            {
                var c = b[0].GetCustomAttributes(typeof(SchematicElementInfo), false);
                if (c.Count() > 0)
                {
                    SchematicElementInfo sei = (SchematicElementInfo)c[0];
                    if (sei != null)
                    {
                        typeDescription = sei.Name;
                    }
                }
            }
            LogData.AddLine("[schematic] " + typeDescription + " added to schematic.");
        }

        public void RunSaveSmithChartImage()
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = "PNG|*.png|BMP|*.bmp|JPEG|*.jpeg,*.jpg";
            sfd.Title = "Export Smith Chart image";

            sfd.ShowDialog();

            if (sfd.FileName != string.Empty)
            {
                LogData.AddLine("[image] Exporting Smith Chart to image \'(" + sfd.FileName + ")\'...");

                string ImExt = Path.GetExtension(sfd.FileName);
                OxyPlot.Wpf.PngExporter.Export(SC.Plot, sfd.FileName, 1024, 768, OxyPlot.OxyColors.White, 300);
                //SvgExporter.Export()

                LogData.AddLine("[image] Done.");
            }
        }

        public static void ActionClear()
        {
            ProgressChanged = null;
            StatusChanged = null;
        }

        private static void ChangeProgress(int progress)
        {
            if (ProgressChanged != null)
                ProgressChanged.Invoke(progress);
        }

        private static void ChangeStatus(StatusType t)
        {
            if (StatusChanged != null)
                StatusChanged.Invoke(t);
        }

        public void SaveProjectToFile(string path, string projectName, string description, double frequency, bool isNormalized, List<SchematicElement> elements)
        {
            LogData.AddLine("[fio] Saving project to file (\"" + path + "\")...");

            using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                sw.WriteLine(HeaderMarker + DateTime.Today.ToString(" MMMM dd, yyyy") + " " + DateTime.Now.ToLongTimeString());
                sw.WriteLine(HeaderMarker + path);
                sw.WriteLine(HeaderMarker + HeaderMarker + " Description");

                if (description != null && description.Length > 0)
                {
                    string[] descriptionStringArray = description.Split('\n');

                    if (descriptionStringArray != null && descriptionStringArray.Length > 0)
                        foreach (string str in descriptionStringArray)
                            if (str != null && str.Length > 1)
                            {
                                while (str.IndexOf('\n') != -1 && str.Length > 0)
                                    str.Remove(str.IndexOf('\n'));

                                sw.Write(HeaderMarker + str);
                            }
                }
                sw.WriteLine();
                sw.WriteLine(HeaderMarker + " Settings");
                sw.WriteLine(DataMarker + "projectName " + projectName);
                sw.WriteLine(DataMarker + "frequency " + frequency);
                sw.WriteLine(DataMarker + "isNormalized " + isNormalized);
                sw.WriteLine(DataMarker + "numElements " + elements.Count());


                if (elements != null && elements.Count > 0)
                    for (int i = 0; i < elements.Count; ++i)
                    {
                        SchematicElement el = elements.ElementAt(i);
                        sw.WriteLine(el.ToStringSimple());

                        if (i % ProgressUpdateIntervall == 0)
                            ChangeProgress((int)(100.0 * i) / elements.Count);
                    }
                ChangeProgress(100);
                Thread.Sleep(FinishedDelay);
                ChangeProgress(0);
                ChangeStatus(StatusType.Ready);
            }

            LogData.AddLine("[fio] Done.");
        }

        public string ReadDescriptionFromFile(string path)
        {
            string ret = string.Empty;

            if (path == string.Empty)
                path = ProjectPath;

            if (!File.Exists(path))
                return ret;

            using (StreamReader sr = File.OpenText(path))
            {
                bool addLines = false;

                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    if (line.Length < 2)
                        continue;

                    if (addLines && line.Substring(0, 2) == HeaderMarker.ToString() )
                        ret += line.Remove(0, 2) + "\n";

                    // found description
                    if (line.Substring(0, 2) == (HeaderMarker.ToString() + HeaderMarker.ToString()))
                    {
                        if (addLines == true)
                            break;
                        addLines = true;
                    }
                }
            }

            return ret;
        }
        public List<SchematicElement> ReadProjectFromFile(string path, out string projectName, out double frequency)
        {
            projectName = "";
            frequency = 0.0;
            return new List<SchematicElement>();
        }

        public List<SchematicElement> ReadProjectFromFile(string path, out string projectName, out double frequency, out bool isNormalized)
        {
            LogData.AddLine("[fio] Reading project file (\"" + path + "\", ...).");

            List<SchematicElement> list = new List<SchematicElement>();
            projectName = "";
            frequency = 0.0;
            isNormalized = false;
            int numElements = 0;

            using (StreamReader sr = File.OpenText(path))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    if ((line.FirstOrDefault() == HeaderMarker) || line == "")
                        continue;

                    //const string dataMarkerString = DataMarker.ToString();
                    string[] data = line.Split(' '); // split line at every whitespace to generate multiple string entries

                    if (line.First() == DataMarker)
                    {
                        string argument = data[1];
                        switch (data[0])
                        {
                            /// TODO: use variable data Marker (problem with const.)
                            case ("!projectName"): projectName = argument; break;

                            case "!frequency": frequency = double.Parse(argument); break;

                            case "!isNormalized": isNormalized = bool.Parse(argument); break;

                            case "!numElements": numElements = int.Parse(argument); break;
                        }
                    }
                    else
                    {
                        list.Add(ElementFromLine(ref data));

                        if (numElements != 0 && list.Count % ProgressUpdateIntervall == 0)
                            ChangeProgress((int)(100.0 * list.Count) / numElements);
                    }
                }
            }

            ChangeProgress(100);
            Thread.Sleep(FinishedDelay);

            LogData.AddLine("[fio] " + list.Count + " Schematic Elements loaded.");

            if (list.Count != numElements)
            {
                MessageBoxResult mbr = MessageBox.Show("Error opening project file (content). Revert project?", "Error opening project file.", MessageBoxButton.YesNo, MessageBoxImage.Error);

                // Revert back to previous state
                if (mbr == MessageBoxResult.Yes)
                    throw new NotImplementedException();
                    //RevertBack();
            }
            return list;
        }

        public void readFromFile(string path)
        {
            List<SchematicElement> list = new List<SchematicElement>();
            string projectName = "";
            double frequency = 0.0;
            bool isNormalized = false;

            list = ReadProjectFromFile(path, out projectName, out frequency, out isNormalized);
            ProjectDescription = ReadDescriptionFromFile(path);
            ProjectPath = path;

            ChangeStatus(StatusType.Ready);
        }

        public SchematicElement ElementFromLine(ref string[] data)
        {
            return new SchematicElement() { Type = SchematicElementType.Port };
        }

        public async void RunTestFeature()
        {
            await Task.Run(() => MessageBox.Show(SC.Frequency.ToString()));
        }

         public void RunShowLogWindow()
        {
            var logWindowViewModel = new LogWindowViewModel(LogData);
        }

        public void RunShowAboutWindow()
        {
            var aboutWindowViewModel = new AboutWindowViewModel();
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
    }

}

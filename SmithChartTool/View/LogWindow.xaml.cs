using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using SmithChartTool.ViewModel;

namespace SmithChartTool.View
{
    /// <summary>
    /// Interaktionslogik für LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        private bool _LogChangedEnabled;
        public LogWindowViewModel VM { get; set; } = new LogWindowViewModel();

        public LogWindow()
        {
            this.DataContext = VM;
            InitializeComponent();

            Log.LogChanged += LogChanged;
            _LogChangedEnabled = true;

            Queue<string> q = Log.GetLines();

            foreach (string line in q)
                rtbLog.AppendText(line + "\r");

            rtbLog.ScrollToEnd();
        }

        ~LogWindow()
        {
            Log.LogChanged -= LogChanged;
            _LogChangedEnabled = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Log.LogChanged -= LogChanged;
            _LogChangedEnabled = false;

            Close();
        }

        delegate void MyDelegate();

        private void LogChanged(string newLogLine)
        {
            MyDelegate dAppend = delegate { rtbLog.AppendText(newLogLine + "\r"); rtbLog.ScrollToEnd(); };

            rtbLog.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, dAppend);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (_LogChangedEnabled == false)
                return;

            Log.AddLine("[log] ### Die Aufzeichnung wurde angehalten. ###\r");

            Log.LogChanged -= LogChanged;
            _LogChangedEnabled = false;

            btnStopLog.IsEnabled = false;
            btnResumeLog.IsEnabled = true;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (_LogChangedEnabled == true)
                return;

            // Bisherigen Text löschen
            rtbLog.Document.Blocks.Clear();

            // Aktuell gespeicherten Log kopieren...
            Queue<string> q = Log.GetLines();

            if (q != null && q.Count > 0)
                //... und erneut ausgeben (Verhindert doppelte Einträge)
                foreach (string line in q)
                    rtbLog.AppendText(line + "\r");

            // An das Ende Scrollen, immer den neuesten Text anzeigen
            rtbLog.ScrollToEnd();

            // LogChanged wieder zulassen
            Log.LogChanged += LogChanged;
            _LogChangedEnabled = true;

            Log.AddLine("[log] ### Die Aufzeichnung wird fortgesetzt. ###\r");

            btnStopLog.IsEnabled = true;
            btnResumeLog.IsEnabled = false;
        }

        private void HandleKeyDownEvent(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
                return;

            if (e.Key == Key.L)
                Close();
        }
    }
}

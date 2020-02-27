using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SmithChartTool.ViewModel
{
    public class Log
    {
        private static Queue<string> LogLines = new Queue<string>();
        public static event Action<string> LogChanged;
        private static int _MaxLogLines = 100;

        public static void AddLine(string newLogString)
        {
            newLogString = "<" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ">: " + newLogString;

            lock (LogLines)
            {
                LogLines.Enqueue(newLogString);

                if (LogLines.Count > _MaxLogLines)
                    LogLines.Dequeue();
            }

            if (LogChanged != null)
                LogChanged.Invoke(newLogString);
        }

        public static Queue<string> GetLines()
        {
            lock (LogLines)
            {
                return new Queue<string>(LogLines);
            }
        }
    }
}

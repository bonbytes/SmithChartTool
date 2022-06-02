using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SmithChartToolLibrary
{
    public class Log
    {
        private ObservableCollection<string> _lines;
        public ObservableCollection<string> Lines
        {
            get
            {
                return _lines;
            }
            set
            {
                if (value != _lines)
                {
                    _lines = value;
                }
            }
        }
        private static int MaxLogLines = 100;

        public Log()
        {
            Lines = new ObservableCollection<string>();
        }

        public void AddLine(string newLogString)
        {
            newLogString = "<" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ">: " + newLogString;

            lock (Lines)
            {
                //Lines.Enqueue(newLogString);
                Lines.Add(newLogString);

                if (Lines.Count > MaxLogLines)
                    //Lines.Dequeue();
                    Lines.RemoveAt(0);
            }
        }
    }
}

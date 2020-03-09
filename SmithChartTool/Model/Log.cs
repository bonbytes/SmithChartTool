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

namespace SmithChartTool.Model
{
    public class Log: INotifyPropertyChanged
    {
        //private Queue<string> _lines;
        private ObservableCollection<string> _lines;
        //public Queue<string> Lines
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
                    OnPropertyChanged("Lines");
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

                OnPropertyChanged("Lines");
            }
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

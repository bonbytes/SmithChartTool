﻿using System;
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
using SmithChartToolApp.ViewModel;
using SmithChartToolLibrary;

namespace SmithChartToolApp.View
{
    /// <summary>
    /// Interaktionslogik für LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        public LogWindow(LogViewModel vm)
        {
            this.DataContext = vm;
            InitializeComponent();
        }

    }
}

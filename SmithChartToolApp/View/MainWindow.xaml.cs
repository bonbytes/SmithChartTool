﻿using System;
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
using SmithChartToolApp.ViewModel;
using System.Windows.Markup;
using OxyPlot;
using System.Threading;
using System.Globalization;

namespace SmithChartToolApp.View
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel vm)
        {
            this.DataContext = vm;
            this.InitializeComponent();

            this.Loaded += (s, e) => 
            {
                List<string> Themes = new List<string>();
                Themes.Add("LightTheme");
                Themes.Add("DarkTheme");
                cmbThemes.DataContext = Themes;

                cmbThemes.SelectionChanged += (_s, _e) =>
                {
                    Application.Current.Resources.MergedDictionaries.Clear();
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/Themes/" + cmbThemes.SelectedItem + ".xaml") });
                };
                cmbThemes.SelectedIndex = 0;
            };
        }
    }
}

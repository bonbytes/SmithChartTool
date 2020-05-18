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
using SmithChartTool.ViewModel;

namespace SmithChartTool.View
{
	/// <summary>
	/// Interaction logic for SettingsWindowView.xaml
	/// </summary>
	public partial class SettingsWindow : Window
	{
		public SettingsWindow(SettingsViewModel vm)
		{
			this.DataContext = vm;
			InitializeComponent();
		}
	}
}
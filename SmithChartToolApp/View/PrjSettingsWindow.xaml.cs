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
using SmithChartToolApp.ViewModel;

namespace SmithChartToolApp.View
{
	/// <summary>
	/// Interaction logic for PrjSettingsWindowView.xaml
	/// </summary>
	public partial class PrjSettingsWindow : Window
	{
		public PrjSettingsWindow(PrjSettingsViewModel vm)
		{
			this.DataContext = vm;
			InitializeComponent();
		}
	}
}

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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmithChartTool.View
{
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class UserControl1 : UserControl
	{
		/*
		 *  // konforme Abbdildung in Matlab / Octave
		    f = @(x) (x - 1) ./ (x + 1);

			close all;
			hold on;

			zero_line = 1E-20

			jrange = logspace(log10(0.15), log10(100), 10)

			# imag. const
			for jmul = [-jrange zero_line jrange]
				plot(f((0:0.1:100)+(j*jmul)))
			endfor

			# real. const
			for roffs = [0 0.2 0.5 1 2 5]
				plot(f(roffs + j*(-100:0.05:100)))
			endfor
		 * 
		 */

		public UserControl1()
		{
			InitializeComponent();
		}
	}
}

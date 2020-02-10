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
	public class MyLabeledControl : ContentControl
	{
		//private Border _brd = null;
		static public DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(MyLabeledControl), new PropertyMetadata(string.Empty));
		static public DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(MyLabeledControl), new PropertyMetadata(Orientation.Horizontal));

		public string Header
		{
			get { return (string)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}

		public Orientation Orientation
		{
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}

		static MyLabeledControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MyLabeledControl), new FrameworkPropertyMetadata(typeof(MyLabeledControl)));
		}

		//public MyLabeledControl()
		//{
		//	this.MouseEnter += new MouseEventHandler(blu);
		//	this.MouseLeave += new MouseEventHandler((s, e) =>
		//	{
		//		if (_brd != null)
		//			_brd.BorderThickness = new Thickness(10);
		//	});
		//}
		//private void blu(object sender, MouseEventArgs e)
		//{
		//	if (_brd != null)
		//		_brd.BorderThickness = new Thickness(1);
		//}

		//public override void OnApplyTemplate()
		//{
		//	base.OnApplyTemplate();
		//	DependencyObject bli = GetTemplateChild("uk"); // UI element out of template
		//	if (bli.GetType() == typeof(Border))
		//	{
		//		_brd = (Border)bli;
		//	}
		//}
	}
}

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
using SmithChartTool.Model;

namespace SmithChartTool.View
{
    public class MySchematicElementControl : Control
    {
        static public DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(SchematicElementType), typeof(MySchematicElementControl), new PropertyMetadata(SchematicElementType.ResistorSerial));

		public SchematicElementType Type
		{
			get { return (SchematicElementType)GetValue(TypeProperty); }
			set { SetValue(TypeProperty, value); }
		}

		static MySchematicElementControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MySchematicElementControl), new FrameworkPropertyMetadata(typeof(MySchematicElementControl)));
        }
    }
}

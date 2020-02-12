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
    public class MySchematicElement : ContentControl
    {
        static public DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(string), typeof(MySchematicElement), new PropertyMetadata(SchematicElementType.ResistorSerial.ToString(), OnTypeChanged));

        public string Type
        {
            get { return (string)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        static MySchematicElement()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MySchematicElement), new FrameworkPropertyMetadata(typeof(MySchematicElement)));
        }

        private void UpdateControl()
        {

        }

        public static void OnTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as MySchematicElement).UpdateControl();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateControl();
        }
    }
}


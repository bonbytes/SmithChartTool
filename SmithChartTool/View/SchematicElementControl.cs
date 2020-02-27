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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SmithChartTool.Model;

namespace SmithChartTool.View
{
    public class SchematicElementControl : ListBoxItem
    {
        static public DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(string), typeof(SchematicElementControl), new PropertyMetadata(string.Empty, OnTypeChanged));
        static public DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(SchematicElementControl), new PropertyMetadata(string.Empty, OnTypeChanged));
        static public DependencyProperty DesignatorProperty = DependencyProperty.Register("Designator", typeof(string), typeof(SchematicElementControl), new PropertyMetadata(string.Empty, OnTypeChanged));
        static public DependencyProperty IsValidInputProperty = DependencyProperty.Register("IsValidInput", typeof(bool), typeof(SchematicElementControl), new PropertyMetadata(true, OnTypeChanged));

        public static DependencyProperty UnitProperty = DependencyProperty.Register("Unit", typeof(string), typeof(SchematicElementControl), new PropertyMetadata(string.Empty));

        public string Unit
        {
            get
            {
                return (string)GetValue(UnitProperty);
            }
            set
            {
                SetValue(UnitProperty, value);
            }
        }

        public string Type
        {
            get { return (string)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public string Designator
        {
            get { return (string)GetValue(DesignatorProperty); }
            set { SetValue(DesignatorProperty, value); }
        }

        public bool IsValidInput
        {
            get { return (bool)GetValue(IsValidInputProperty); }
            set { SetValue(IsValidInputProperty, value); }
        }

        static SchematicElementControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SchematicElementControl), new FrameworkPropertyMetadata(typeof(SchematicElementControl)));
        }

        private void UpdateControl(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var elementData = sender.GetValue(DataContextProperty);
            var t = ((SchematicElement)elementData).Type.GetType();
            var attributeData = t.GetMember((((SchematicElement)elementData).Type).ToString());
            var attributes = attributeData[0].GetCustomAttributes(typeof(SchematicElementInfo), false);
            SchematicElementInfo sei = (SchematicElementInfo)attributes[0];

            var sri = Application.GetResourceStream(new Uri("pack://application:,,,/Images/SchematicElements/" + sei.Icon + ".xaml"));
            var content = XamlReader.Load(sri.Stream);
            Content = content;

            if( ((SchematicElement)elementData).Type == SchematicElementType.Port )
            {
                Value = (((SchematicElement)elementData).Impedance.ToString() + " Ohms");
            }
            else
            {
                Value = ((SchematicElement)elementData).Value.ToString();
            }
            
            Designator = sei.Designator + ((SchematicElement)elementData).Designator.ToString();
        }

        public static void OnTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as SchematicElementControl).UpdateControl(sender, e);
        }

        //public override void OnApplyTemplate()
        //{
        //    base.OnApplyTemplate();
        //    UpdateControl();
        //}
    }
}


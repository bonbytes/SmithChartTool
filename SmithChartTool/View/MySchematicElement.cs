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
    public class MySchematicElement : ContentControl
    {
        static public DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(string), typeof(MySchematicElement), new PropertyMetadata(SchematicElementType.ResistorSerial.ToString(), OnTypeChanged));
        static public DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(MySchematicElement), new PropertyMetadata(string.Empty, OnTypeChanged));
        static public DependencyProperty DesignatorProperty = DependencyProperty.Register("Designator", typeof(string), typeof(MySchematicElement), new PropertyMetadata(string.Empty, OnTypeChanged));

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

        static MySchematicElement()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MySchematicElement), new FrameworkPropertyMetadata(typeof(MySchematicElement)));
        }

        private void UpdateControl()
        {
            var a = typeof(SchematicElementType).FromName(Type);
            if (a != null)
            {
                Type t = a.GetType();
                var b = t.GetMember(a.ToString());

                if (b.Count() > 0)
                {
                    var c = b[0].GetCustomAttributes(typeof(SchematicElementInfo), false);
                    if (c.Count() > 0)
                    {

                        SchematicElementInfo sei = (SchematicElementInfo)c[0];
                        //Header = sei.Name;
                        //img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/SchematicElements/"+ sei.Icon +".png"));
                        if (sei != null)
                        {
                            var sri = Application.GetResourceStream(new Uri("pack://application:,,,/Images/SchematicElements/" + sei.Icon + ".xaml"));
                            var content = XamlReader.Load(sri.Stream);
                            Content = content;
                        }
                    }
                }
            }

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


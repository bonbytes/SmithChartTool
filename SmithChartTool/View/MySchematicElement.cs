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
    public class MySchematicElement : Control
    {
        private Image img = null;
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

        private void UpdateImage()
        {
            if (img != null)
            {
                var a = typeof(SchematicElementType).FromName(Type);
                switch (a)
                {
                    case SchematicElementType.Port1:
                        img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/SchematicElements/Port1.png"));
                        break;
                    case SchematicElementType.Port2:
                        img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/SchematicElements/Port2.png"));
                        break;
                    case SchematicElementType.ResistorSerial:
                        img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/SchematicElements/ResistorSerial.png"));
                        break;
                    case SchematicElementType.ResistorParallel:
                        img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/SchematicElements/ResistorParallel.png"));
                        break;
                    case SchematicElementType.CapacitorSerial:
                        img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/SchematicElements/CapacitorSerial.png"));
                        break;
                    case SchematicElementType.CapacitorParallel:
                        img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/SchematicElements/CapacitorParallel.png"));
                        break;
                    case SchematicElementType.InductorSerial:
                        img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/SchematicElements/InductorSerial.png"));
                        break;
                    case SchematicElementType.InductorParallel:
                        img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/SchematicElements/InductorParallel.png"));
                        break;
                    case SchematicElementType.TLine:
                        img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/SchematicElements/TLine.png"));
                        break;
                    case SchematicElementType.OpenStub:
                        img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/SchematicElements/OpenStub.png"));
                        break;
                    case SchematicElementType.ShortedStub:
                        img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/SchematicElements/ShortedStub.png"));
                        break;
                    default:
                        img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/SchematicElements/Default.png"));
                        break;
                }
            }
        }

        public static void OnTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as MySchematicElement).UpdateImage();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //DependencyObject b = GetTemplateChild("PART_MySchematicElementImage"); // UI element out of template
            //if (b != null && (b.GetType() == typeof(Image)))
            //{
            //    img = b as Image;
            //    UpdateImage();
            //}
        }
    }
}


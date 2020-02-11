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
        private Image img = null;
        static public DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(string), typeof(MySchematicElementControl), new PropertyMetadata(SchematicElementType.InductorSerial.ToString(), OnTypeChanged));
      

        public string Type
        {
            get { return (string)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        static MySchematicElementControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MySchematicElementControl), new FrameworkPropertyMetadata(typeof(MySchematicElementControl)));
        }

        private void UpdateImage()
        {
            if(img != null)
            {
                var a = typeof(SchematicElementType).FromName(Type);
                //if (a.GetType() != typeof(string))
                    
                    //a = a.Type.ToString();
                switch (a)
                {
                    case SchematicElementType.CapacitorParallel:
                        img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/SchematicElements/CapacitorParallel.png"));
                        break;
                    

                    default:
                        img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/SchematicElements/ResistorParallel.png"));
                        break;
                }
            }
        }

        public static void OnTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as MySchematicElementControl).UpdateImage();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DependencyObject b = GetTemplateChild("PART_MySchematicElementControlImage"); // UI element out of template
            if(b != null && (b.GetType() == typeof(Image)))
            {
                img = b as Image;
                UpdateImage();
            }
        }
    }
}

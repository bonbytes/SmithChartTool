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
        static public DependencyProperty ImagePathProperty = DependencyProperty.Register("ImagePath", typeof(string), typeof(MySchematicElementControl), new PropertyMetadata((ImageSource)converter.ConvertFromString("pack://application:,,,/Images/SchematicElements/ResistorParallel.png")));

        static public ImageSourceConverter converter = new ImageSourceConverter();

        public ImageSource ImagePath
        {
            get { return (ImageSource)GetValue(ImagePathProperty); }
            set { SetValue(ImagePathProperty, (converter.ConvertFromString("pack://application:,,,/Images/SchematicElements/" + value + ".png"))); }
        }

        //public string ImagePath
        //{
        //    get { return (string)GetValue(ImagePathProperty); }
        //    set { SetValue(ImagePathProperty, (string)("pack://application:,,,/Images/SchematicElements/" + value + ".png")); }
        //}

        static MySchematicElementControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MySchematicElementControl), new FrameworkPropertyMetadata(typeof(MySchematicElementControl)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DependencyObject b = GetTemplateChild("MySchematicElementControlImage"); // UI element out of template
            if (b.GetType() == typeof(Image))
            {
                (b as Image).Source = ImagePath;
                //(b as Image).Source = (ImageSource)converter.ConvertFromString(this.ImagePath);
            }
        }
    }
}

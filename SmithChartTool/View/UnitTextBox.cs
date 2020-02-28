using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmithChartTool.View
{
    public class UnitTextBox : TextBox
    {

        public static DependencyProperty UnitProperty = DependencyProperty.Register("Unit", typeof(string), typeof(UnitTextBox), new PropertyMetadata(string.Empty));

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

        private string Prefix
        {
            get; set;
        }

        static UnitTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UnitTextBox), new FrameworkPropertyMetadata(typeof(UnitTextBox)));
        }
        public UnitTextBox()
        {
            TextChanged += new TextChangedEventHandler(MyTextChanged);
        }

        private void MyTextChanged(object sender, TextChangedEventArgs e)
        {
            if (Text == string.Empty)
                Text += Unit;

            else if (Text.IndexOf(Unit) == -1)
            {
                int tmp = ((TextBox)e.Source).SelectionStart; // get current cursor position
                Text += ' ' + Unit; // append unit and reset cursor position
                ((TextBox)e.Source).SelectionStart = tmp; // restore cursor
            }
        }    
    }
}

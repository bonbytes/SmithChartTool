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
    public class UnitTextBoxControl : TextBox
    {

        public static DependencyProperty UnitProperty = DependencyProperty.Register("Unit", typeof(string), typeof(UnitTextBoxControl), new PropertyMetadata(string.Empty));

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

        static UnitTextBoxControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UnitTextBoxControl), new FrameworkPropertyMetadata(typeof(UnitTextBoxControl)));
        }
        public UnitTextBoxControl()
        {
            TextChanged += new TextChangedEventHandler(MyTextChanged);
        }

        private void MyTextChanged(object sender, TextChangedEventArgs e)
        {
            if (Text == "")
                Text = Unit;

            else if (Text.IndexOf(Unit, StringComparison.Ordinal) == -1)
            {
                int tmp = ((TextBox)e.Source).SelectionStart;
                //Text += ' ' + Unit; // append unit and reset cursor position
                ((TextBox)e.Source).SelectionStart = tmp; // restore cursor
            }
        }    
    }
}

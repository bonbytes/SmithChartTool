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
            if (Text.IndexOf(Unit) == -1)
            {
                int tmp = ((TextBox)e.Source).SelectionStart; // get current cursor position
                Text += Unit; // append unit and reset cursor position
                ((TextBox)e.Source).SelectionStart = tmp; // restore cursor
            }
        }

        private static void SetPrefix(string unit, TextBox textbox)
        {
            if (textbox.Text.ToLower().IndexOf(unit.ToLower()) == -1)
            {
                int tmp = textbox.SelectionStart; // get current cursor position
                textbox.Text += ' ' + unit; // append unit and reset cursor position
                textbox.SelectionStart = tmp; // save cursor
            }
        }

        public static double ToDouble(string str)
        {
            try
            {
                int indexChar = 0;

                while (indexChar < str.Length && (char.IsDigit(str[indexChar]) || str[indexChar] == '.' || str[indexChar] == 'E' || str[indexChar] == '-'))
                    indexChar++;

                string numString = str.Substring(0, indexChar);

                if (numString.Length == 0)
                    return 0;

                // skip whitespaces between number and prefix
                while (indexChar < str.Length && char.IsWhiteSpace(str[indexChar]))
                    indexChar++;

                string prefixUnit = (str.Substring(indexChar));
                double num = double.Parse(numString);

                switch (prefixUnit)
                {
                    case "f":
                        return num / 1000000000000;
                    case "p":
                        return num / 1000000000;
                    case "µ":
                        return num / 1000000;
                    case "m":
                        return num / 1000;
                    case "c":
                        return num / 100;
                    case "d":
                        return num / 10;
                    case "k":
                        return num * Math.Pow(10, 3);
                    case "M":
                        return num * Math.Pow(10, 6);
                    case "G":
                        return num * Math.Pow(10, 9);
                    case "T":
                        return num * Math.Pow(10, 12);

                    default:
                        return num;
                }
            }
            catch (FormatException fe)
            {
                if (str.IndexOf('E') >= 1 && char.IsNumber(str[str.IndexOf('E') - 1]))
                    return 0;

                // No number recognized. set to zero
                MessageBox.Show("Fehler! Eingabe '" + str + "' nicht erkannt!"
                                + "\nSetze auf 0. (" + fe.Message + ")",
                                "Eingabe nicht erkannt", MessageBoxButton.OK, MessageBoxImage.Information);

                return 0;
            }
        }
    
}
}

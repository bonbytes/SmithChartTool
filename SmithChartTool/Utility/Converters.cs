using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

//	already implemented converters:
//    Derived from IValueConverter

//System.Windows.Controls.AlternationConverter
//System.Windows.Controls.BooleanToVisibilityConverter
//System.Windows.Documents.ZoomPercentageConverter
//System.Windows.Navigation.JournalEntryListConverter

//  Derived from IMultiValueConverter
//System.Windows.Controls.BorderGapMaskConverter
//System.Windows.Navigation.JournalEntryUnifiedViewConverter
//System.Windows.Controls.MenuScrollingVisibilityConverter

//Microsoft.Windows.Themes.ProgressBarBrushConverter
//Microsoft.Windows.Themes.ProgressBarHighlightConverter

namespace SmithChartTool.Utility
{
    public class TextBoxValueConverter : IValueConverter
    {
        // Backend -> Frontend
        // attributes here?
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = value.ToString();
            s = s.Replace(".", ",");
            return s;
        }

        // Frontend -> Backend
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                string str = value as string;
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

                    string prefix = (str.Substring(indexChar));
                    double num = double.Parse(numString);

                    switch (prefix)
                    {
                        case "f":
                            return num / Math.Pow(10, 12);
                        case "p":
                            return num / Math.Pow(10, 9);
                        case "µ":
                        case "u":
                            return num / Math.Pow(10, 6);
                        case "m":
                            return num / Math.Pow(10, 3);
                        case "c":
                            return num / 100;
                        case "d":
                            return num / 10;
                        case "k":
                            return num * Math.Pow(10, 3);
                        case "M":
                        case "Meg":
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
                    MessageBox.Show(str + "not recognized"
                                    + "\n Setting value to zero.",
                                    "Eingabe nicht erkannt", MessageBoxButton.OK, MessageBoxImage.Information);
                    return 0;
                }
            }
            else
                return 0;
        }
    }

    public class TextBoxToComplex32Converter : IValueConverter
    {
        // Backend -> UI
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            char[] charsToTrim = { '(', ')' };
            var val = (Complex32)value;
            string compstring = val.ToString();
            return compstring.Trim(charsToTrim);

        }
        // UI -> Backend
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                Complex32 compvalue = new Complex32();
                string compstring = (string)value;
                if (Complex32.TryParse(compstring.Replace(" ", string.Empty), out compvalue))
                    return new Complex32(compvalue.Real, compvalue.Imaginary);
            }
            return null;
        }
    }

    public class BooleanInvertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // throw new NotImplementedException();
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            return !(bool)value;
        }
    }

    public class BooleanToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value == true)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // return value == Visibility.Visible;
            return value;
        }
    }

    public class BooleanToVisibilityInvertedConverter : IValueConverter
    {

        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //return value == Visibility.Visible ? false : true;
            return value;
        }
    }

    public class YesNoToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch (value.ToString().ToLower())
            {
                case "yes":
                    return true;
                case "no":
                    return false;
            }
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value == true)
                    return "yes";
                else
                    return "no";
            }
            return "no";
        }
    }

    public class PlotSizeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values == null || values.Length != 2)
            {
                throw new ArgumentException("Values darf nicht Null oder die Länge ungleich 2 sein","values");
            }
            if ((double)values[0] > (double)values[1])
            {
                return (double)values[1];
            }
            else
            {
                return (double)values[0];
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}

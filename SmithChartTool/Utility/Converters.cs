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
            if(value != null)
            {
                string s = value.ToString();
                s = s.Replace(",", ".");
                return s;
            }
            return null;
        }

        // Frontend -> Backend
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                string str = value as string;
                try
                {
                    
                    return Formatters.StringPrefixToDouble(num, prefix);
                }
                catch (FormatException)
                {
                    if (str.IndexOf('E') >= 1 && char.IsNumber(str[str.IndexOf('E') - 1]))
                        return null;

                    return null;
                }
            }
            else
                return null;
        }
    }

    public class StringToComplex32Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value is Complex32)
            {
                return Formatters.Complex32ToString((Complex32)value); 
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                Complex32 compvalue;
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
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
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

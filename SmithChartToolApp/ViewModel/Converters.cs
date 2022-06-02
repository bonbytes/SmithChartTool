using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

// Derived from IValueConverter
//System.Windows.Controls.AlternationConverter
//System.Windows.Documents.ZoomPercentageConverter
//System.Windows.Navigation.JournalEntryListConverter

// Derived from IMultiValueConverter
//System.Windows.Controls.BorderGapMaskConverter
//System.Windows.Navigation.JournalEntryUnifiedViewConverter
//System.Windows.Controls.MenuScrollingVisibilityConverter
//Microsoft.Windows.Themes.ProgressBarBrushConverter
//Microsoft.Windows.Themes.ProgressBarHighlightConverter

namespace SmithChartToolApp.ViewModel
{
    /// <summary>
    /// DoubleToStringConverter
    /// Used for Binding double values to TextBox.Text
    /// </summary>
    public class DoubleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double && value != null)
            {
                var amount = SIPrefix.GetInfo((double)value, 2);
                return amount.AmountWithPrefix;
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string && value != null)
            {
                try
                {
                    return SIPrefix.GetValue((string)value);
                }
                catch (FormatException)
                {
                    return null;
                }
            }
            else
                return null;
        }
    }

    /// <summary>
    /// SchematicElementValueToStringConverter
    /// Used for binding SchematicElement.Values to TextBox.Text
    /// </summary>
    public class SchematicElementValueToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Test for Port -> use Complex32ToStringConverter
            if (value is string && value != null)
            {
                var amount = SIPrefix.GetInfo(SIPrefix.GetValue((string)value), 2);
                return amount.AmountWithPrefix;
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string && value != null)
            {
                try
                {
                    var amount = SIPrefix.GetInfo(SIPrefix.GetValue((string)value), 2);
                    return amount.AmountWithPrefix;
                }
                catch (FormatException)
                {
                    return null;
                }
            }
            else
                return null;
        }
    }

    /// <summary>
    /// Complex32ToStringConverter
    /// Used for binding Complex32 values to TextBox.Text
    /// </summary>
    public class Complex32ToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Complex32)
            {
                return Formatters.Complex32ToString((Complex32)value);
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
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

    /// <summary>
    /// BooleanInvertConverter
    /// Used for inversion of binded boolean values
    /// </summary>
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

    /// <summary>
    /// BooleanToVisibilityConverter
    /// Used for binding of boolean values to Visibility properties
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value == true)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // return value == Visibility.Visible;
            return value;
        }
    }

    /// <summary>
    /// BooleanToVisibilityInvertedConverter
    /// Used for binding of boolean values to inverted Visibility properties
    /// </summary>
    public class BooleanToVisibilityInvertedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //return value == Visibility.Visible ? false : true;
            return value;
        }
    }

    /// <summary>
    /// YesNoToBooleanConverter
    /// Used for binding of "yes", "no" choices to boolean values
    /// </summary>
    public class YesNoToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
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

    /// <summary>
    /// PlotSizeConverter
    /// Used for square SmithChart plot size
    /// </summary>
    public class PlotSizeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2)
            {
                throw new ArgumentException("Values darf nicht Null oder die Länge ungleich 2 sein", "values");
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

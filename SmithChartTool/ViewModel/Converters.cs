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



namespace SmithChartTool.ViewModel
{
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
    } // end class BooleanToVisibilityConverter



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
    } // end class BooleanToVisibilityInvertedConverter



    public class BooleanToGridLengthConverter : IValueConverter
    {

        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((GridLength)value).Value == 0 ? false : true;
        }
    } // end class BooleanToGridLengthConverter



    public class YesNoToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch (value.ToString().ToLower())
            {
                case "yes":
                case "oui":
                case "ja":
                case "jup":
                    return true;
                case "no":
                case "non":
                case "nein":
                case "nö":
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
    } // end class YesNoToBooleanConverter

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

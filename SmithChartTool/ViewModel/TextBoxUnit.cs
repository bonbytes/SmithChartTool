using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace SmithChartTool.ViewModel
{
    public class TextBoxUnit
    {
        public static void SetUnit(string unit, TextBox textbox)
        {
            if (textbox.Text.ToLower().IndexOf(unit.ToLower()) == -1)
            {
                int tmp = textbox.SelectionStart; // get current cursor position
                textbox.Text += ' ' + unit; // append unit and reset cursor position
                textbox.SelectionStart = tmp; // save cursor
            }
        }

        //public static string SetUnit(string[] units, TextBox textbox, string prefUnit = "mm")
        //{
        //    if (textbox.Text == "")
        //        return prefUnit;

        //    foreach (string unit in units)
        //        if (textbox.Text.ToLower().IndexOf(unit.ToLower()) != -1)
        //            return unit;

        //    int tmp = textbox.SelectionStart;
        //    textbox.Text += ' ' + prefUnit;
        //    textbox.SelectionStart = tmp;

        //    return prefUnit;
        //}

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


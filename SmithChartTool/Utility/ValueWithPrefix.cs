using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmithChartTool.Utility
{
    public class ValueWithPrefix
    {
        public double Value { get; set; }
        public string Prefix { get; set; }

        public ValueWithPrefix()
        {
            Value = 0.0;
            Prefix = string.Empty;
        }

        public ValueWithPrefix(double value, string prefix = "")
        {
            Value = value;
            Prefix = prefix;
        }

        public void SplitInputString(string str)
        {
            int indexChar = 0;

            while (indexChar < str.Length && (char.IsDigit(str[indexChar]) || str[indexChar] == '.' || str[indexChar] == 'E' || str[indexChar] == 'e' || str[indexChar] == '-'))
                indexChar++;

            string numString = str.Substring(0, indexChar); // "number" ranges to first non-digit (excluding modifier keys, seen above)

            if (numString.Length == 0)
            {
                Value = 0;
                Prefix = string.Empty;
            }
            // skip whitespaces between number and prefix
            while (indexChar < str.Length && char.IsWhiteSpace(str[indexChar]))
                indexChar++;

            Value = double.Parse(numString);
            Prefix = str.Substring(indexChar);
        }
    }
}

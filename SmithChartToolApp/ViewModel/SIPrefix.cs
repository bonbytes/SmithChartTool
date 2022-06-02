using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmithChartToolApp.ViewModel
{
    public class SIPrefixInfo : ICloneable
    {
        public string Symbol { get; set; }
        public decimal Example { get; set; }
        public string Prefix { get; set; }
        public int ZeroLength { get; set; }
        public string ShortScaleName { get; set; }
        public string LongScaleName { get; set; }
        public string AmountWithPrefix { get; set; }

        public object Clone()
        {
            return new SIPrefixInfo()
            {
                Example = Example,
                LongScaleName = LongScaleName,
                ShortScaleName = ShortScaleName,
                Symbol = Symbol,
                Prefix = Prefix,
                ZeroLength = ZeroLength
            };

        }
    }

    public static class SIPrefix
    {
        private static List<SIPrefixInfo> _SIPrefixInfoList = new List<SIPrefixInfo>();
        public static List<SIPrefixInfo> SIPrefixInfoList
        {
            get
            {
                SIPrefixInfo[] siPrefixInfoList = new SIPrefixInfo[6];
                _SIPrefixInfoList.CopyTo(siPrefixInfoList);
                return siPrefixInfoList.ToList();
            }
        }

        static SIPrefix()
        {
            _SIPrefixInfoList = new List<SIPrefixInfo>();
            LoadSIPrefix();
        }

        private static void LoadSIPrefix()
        {
            _SIPrefixInfoList.AddRange(new SIPrefixInfo[]
            {
                new SIPrefixInfo() {Symbol = "P", Prefix = "peta", Example = 1000000000000000M, ZeroLength = 15, ShortScaleName = "Quadrillion", LongScaleName = "Billiard"},
                new SIPrefixInfo() {Symbol = "T", Prefix = "tera", Example = 1000000000000M, ZeroLength = 12, ShortScaleName = "Trillion", LongScaleName = "Billion"},
                new SIPrefixInfo() {Symbol = "G", Prefix = "giga", Example = 1000000000M, ZeroLength = 9, ShortScaleName = "Billion", LongScaleName = "Milliard"},
                new SIPrefixInfo() {Symbol = "M", Prefix = "mega", Example = 1000000M, ZeroLength = 6, ShortScaleName = "Million", LongScaleName = "Million"},
                new SIPrefixInfo() {Symbol = "k", Prefix = "kilo", Example = 1000M, ZeroLength = 3, ShortScaleName = "Thousand", LongScaleName = "Thousand"},
                new SIPrefixInfo() {Symbol = "h", Prefix = "hecto", Example = 100M, ZeroLength = 2, ShortScaleName = "Hundred", LongScaleName = "Hundred"},
                new SIPrefixInfo() {Symbol = "da", Prefix = "deca", Example = 10M, ZeroLength = 1, ShortScaleName = "Ten", LongScaleName = "Ten"},
                new SIPrefixInfo() {Symbol = "", Prefix = "", Example = 1M, ZeroLength = 0, ShortScaleName = "One", LongScaleName = "One"},
                new SIPrefixInfo() {Symbol = "d", Prefix = "deci", Example = 10M, ZeroLength = -1, ShortScaleName = "Tenth", LongScaleName = "Tenth"},
                new SIPrefixInfo() {Symbol = "c", Prefix = "centi", Example = 10M, ZeroLength = -2, ShortScaleName = "Hundreth", LongScaleName = "Hundreth"},
                new SIPrefixInfo() {Symbol = "m", Prefix = "milli", Example = 0.001M, ZeroLength = -3, ShortScaleName = "Thousandth", LongScaleName = "Thousandt"},
                new SIPrefixInfo() {Symbol = "u", Prefix = "micro", Example = 0.000001M, ZeroLength = -6, ShortScaleName = "Millionth", LongScaleName = "Millionth"},
                new SIPrefixInfo() {Symbol = "n", Prefix = "nano", Example = 0.000000001M, ZeroLength = -9, ShortScaleName = "Billionth", LongScaleName = "Milliardth"},
                new SIPrefixInfo() {Symbol = "p", Prefix = "pico", Example = 0.000000000001M, ZeroLength = -12, ShortScaleName = "Trillionth", LongScaleName = "Billionth"},
                new SIPrefixInfo() {Symbol = "f", Prefix = "femto", Example = 0.000000000000001M, ZeroLength = -15, ShortScaleName = "Quadrillionth", LongScaleName = "Billiardth"},
            });
        }

        public static SIPrefixInfo GetInfo(double amount, int decimals)
        {
            return GetInfo(Convert.ToDecimal(amount), decimals);
        }

        public static SIPrefixInfo GetInfo(decimal amount, int decimals)
        {
            SIPrefixInfo siPrefixInfo = null;
            decimal amountToTest = Math.Abs(amount);

            if (amountToTest == 0)
            {
                siPrefixInfo = _SIPrefixInfoList.Find(i => i.ZeroLength == 0).Clone() as SIPrefixInfo;
                siPrefixInfo.AmountWithPrefix = Math.Round(amount, decimals).ToString();
                return siPrefixInfo;
            }

            //var amountLength = amountToTest.ToString("{0}").Length;
            var amountLength = Math.Abs(Math.Floor(Math.Log10((double)amountToTest) + 1));
            if (amountLength < 3)
            {
                siPrefixInfo = _SIPrefixInfoList.Find(i => i.ZeroLength == amountLength).Clone() as SIPrefixInfo;
                siPrefixInfo.AmountWithPrefix = Math.Round(amount, decimals).ToString();

                return siPrefixInfo;
            }

            siPrefixInfo = _SIPrefixInfoList.Find(i => amountToTest > i.Example).Clone() as SIPrefixInfo;
            siPrefixInfo.AmountWithPrefix = Math.Round(amountToTest / Convert.ToDecimal(siPrefixInfo.Example), decimals).ToString() + siPrefixInfo.Symbol;

            return siPrefixInfo;
        }

        public static double GetValue(string str)
        {
            int indexChar = 0;

            while (indexChar < str.Length && (char.IsDigit(str[indexChar]) || str[indexChar] == '.' || str[indexChar] == ' ' || str[indexChar] == 'E' || str[indexChar] == 'e' || str[indexChar] == '-'))
                indexChar++;

            string numString = str.Substring(0, indexChar); // "number" ranges to first non-digit (excluding modifier keys, seen above)
            numString = Regex.Replace(numString, @"\s+", "").Trim();

            if (numString.Length == 0)
            {
                return 0.0;
            }
            // skip whitespaces between number and prefix
            while (indexChar < str.Length && char.IsWhiteSpace(str[indexChar]))
                indexChar++;

            double num = double.Parse(numString);
            string prefix = str.Substring(indexChar);

            if (prefix == string.Empty)
            {
                return num;
            }
            switch (prefix)
            {
                case "f":
                    return num / Math.Pow(10, 15);
                case "p":
                    return num / Math.Pow(10, 12);
                case "n":
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
                    throw new ArgumentException();
            }
        }
    }
}

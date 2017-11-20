using IisLogArchiver.Interfaces;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace IisLogArchiver.FileHandling
{
    public class FileNameParser : IFileNameParser
    {
        public bool TryParseDateFromString(string str, out DateTime outdt)
        {
            if (string.IsNullOrEmpty(str) || !(str.Contains(".log") || str.Contains(".txt")))
            {
                outdt = default(DateTime);
                return false;
            }

            // many edge cases left wide open. this is good enough for now
            try
            {
                //only the numbers from the filename
                var digits = Regex.Replace(str, "[^0-9]", "");
                // assuming the first digits from the filename is the date in format yyMMdd or yyyyMMdd
                var dateStr = "";
                if (digits.Length > 7)
                    dateStr = digits.StartsWith("20") ? digits.Substring(2, 6) : digits.Substring(0, 6);
                else if (digits.Length > 5)
                    dateStr = digits.Substring(0, 6);

                outdt = DateTime.ParseExact(dateStr, "yyMMdd", CultureInfo.InvariantCulture);
                return true;
            }
            catch (Exception)
            {
                outdt = default(DateTime);
            }
            return false;
        }
    }
}
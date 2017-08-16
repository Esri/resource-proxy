using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP.Cloud.OnlineRateTable.Common
{
    public class NumberConverter
    {
        public delegate bool TryParseDelegate<T>(string s, out T result);
        public delegate T ParseDelegate<T>(string s, NumberStyles style);

        public static T TryParse<T>(string value, TryParseDelegate<T> parse) where T : struct
        {
            T result = default(T);
            parse(value, out result);
            return result;
        }

        public static T Parse<T>(string value, NumberStyles style, ParseDelegate<T> parse) where T : struct
        {
            try
            {
                return parse(value, style);
            }
            catch(Exception)
            {
                return default(T);
            }
        }
    }
}

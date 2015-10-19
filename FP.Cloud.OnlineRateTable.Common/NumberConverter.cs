using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP.Cloud.OnlineRateTable.Common
{
    public class NumberConverter
    {
        public delegate bool ParseDelegate<T>(string s, out T result);

        public static T TryParse<T>(string value, ParseDelegate<T> parse) where T : struct
        {
            T result = default(T);
            parse(value, out result);
            return result;
        }
    }
}

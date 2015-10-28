using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    [DataContract]
    public class PostageInfo
    {
        #region properties
        [DataMember]
        public UInt64 PostageValue { get; set; }
        [DataMember]
        public ushort PostageDecimals { get; set; }
        [DataMember]
        public string CurrencySymbol { get; set; }
        [DataMember]
        public string CurrencyDecimalSeparator { get; set; }
        #endregion

        #region public
        public string GetFormattedPostage(bool includeCurrency, bool threeDigit)
        {
            ulong major = (ulong)(PostageValue / Math.Pow(10, PostageDecimals));
            ulong minor = (ulong)(PostageValue % Math.Pow(10, PostageDecimals));
            string separator = string.IsNullOrEmpty(CurrencyDecimalSeparator) ? "." : CurrencyDecimalSeparator;

            string postageString = threeDigit ? string.Format("{0}{1}{2}", major, separator, minor.ToString("000")) :
                                                string.Format("{0}{1}{2}", major, separator, minor.ToString("00"));
            if (includeCurrency)
            {
                return string.Format("{0}{1}", CurrencySymbol, postageString);
            }
            return postageString;
        }
        #endregion
    }
}

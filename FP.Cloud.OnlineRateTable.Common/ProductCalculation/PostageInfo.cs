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
        #endregion

        #region public
        public string GetFormattedPostage(CultureInfo culture, bool includeCurrency, bool threeDigit)
        {
            decimal postage = (decimal)PostageValue / (decimal)Math.Pow(10, PostageDecimals);
            string postageString = threeDigit ? postage.ToString("0.000", culture) : postage.ToString("0.00", culture);
            if (includeCurrency)
            {
                return string.Format("{0}{1}", CurrencySymbol, postageString);
            }
            return postageString;
        }
        #endregion
    }
}

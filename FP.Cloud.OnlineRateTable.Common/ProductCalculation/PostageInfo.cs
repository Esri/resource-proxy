using System;
using System.Runtime.Serialization;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    [DataContract]
    public class PostageInfo
    {
        [DataMember]
        public UInt64 PostageValue { get; set; }
        [DataMember]
        public ushort PostageDecimals { get; set; }
        [DataMember]
        public string CurrencySymbol { get; set; }
    }
}

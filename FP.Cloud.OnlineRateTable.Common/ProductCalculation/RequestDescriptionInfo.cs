using System;
using System.Runtime.Serialization;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    [DataContract]
    public class RequestDescriptionInfo : DescriptionInfo
    {
        [DataMember]
        public string StatusMessage { get; set; }
        [DataMember]
        public int Label { get; set; }
        [DataMember]
        public string DisplayFormat { get; set; }
    }
}

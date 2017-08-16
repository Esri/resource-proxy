using System;
using System.Runtime.Serialization;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    [DataContract]
    public class EnvironmentInfo
    {
        [DataMember]
        public string Iso3CountryCode { get; set; }
        [DataMember]
        public int CarrierId { get; set; }
        [DataMember]
        public DateTime UtcDate { get; set; }
        [DataMember]
        public string Culture { get; set; }
        [DataMember]
        public string SenderZipCode { get; set; }
    }
}

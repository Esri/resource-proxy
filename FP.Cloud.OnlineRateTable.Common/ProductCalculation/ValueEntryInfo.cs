using System;
using System.Runtime.Serialization;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    [DataContract]
    public class ValueEntryInfo
    {
        [DataMember]
        public int EntryValue { get; set; }
        [DataMember]
        public string EntryMessage { get; set; }
    }
}

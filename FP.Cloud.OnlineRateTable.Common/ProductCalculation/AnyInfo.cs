using System;
using System.Runtime.Serialization;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    [DataContract]
    public class AnyInfo
    {
        [DataMember]
        public EAnyType AnyType { get; set; }
        [DataMember]
        public string AnyValue { get; set; }
    }
}

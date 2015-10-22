using System;
using System.Runtime.Serialization;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    [DataContract]
    public class PCalcResultInfo
    {
        [DataMember]
        public ProductDescriptionInfo ProductDescription { get; set; }
        [DataMember]
        public DescriptionInfo QueryDescription { get; set; }
        [DataMember]
        public EQueryType QueryType { get; set; }
    }
}

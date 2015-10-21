using System;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    public class PCalcResultInfo
    {
        public ProductDescriptionInfo ProductDescription { get; set; }
        public DescriptionInfo QueryDescription { get; set; }
        public EQueryType QueryType { get; set; }
    }
}

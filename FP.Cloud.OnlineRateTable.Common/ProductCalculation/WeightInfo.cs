using System;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    public class WeightInfo
    {
        public int WeightValue { get; set; }
        public EWeightUnit WeightUnit { get; set; }
    }
}

using System;
using System.Runtime.Serialization;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    public class WeightInfo
    {
        public int WeightValue { get; set; }
        public EWeightUnit WeightUnit { get; set; }

        [IgnoreDataMember]
        public string FormattedWeight
        {
            get { return string.Empty; }
        }

        [IgnoreDataMember]
        public string FormattedWeightImperial
        {
            get { return string.Empty; }
        }
    }
}

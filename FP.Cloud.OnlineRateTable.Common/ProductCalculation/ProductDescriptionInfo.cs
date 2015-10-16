using System.Collections.Generic;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    public class ProductDescriptionInfo
    {
        #region properties
        public PostageInfo Postage { get; set; }
        public int ProductId { get; set; }
        public EScaleMode ScaleMode { get; set; }
        public EProductDescriptionState State { get; set; }
        public WeightInfo Weight { get; set; }
        public int WeightClass { get; set; }
        List<AttributeInfo> Attributes { get; set; }
        List<string> ReadyModeSelection { get; set; }
        public int ProductCode { get; set; }
        public int RateVersion { get; set; }
        #endregion

        #region constructor
        public ProductDescriptionInfo()
        {
            Attributes = new List<AttributeInfo>();
            ReadyModeSelection = new List<string>();
        }
        #endregion
    }
}

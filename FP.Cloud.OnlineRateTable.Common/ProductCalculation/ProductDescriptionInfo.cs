using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    [DataContract]
    public class ProductDescriptionInfo
    {
        #region properties
        [DataMember]
        public PostageInfo Postage { get; set; }
        [DataMember]
        public int ProductId { get; set; }
        [DataMember]
        public EScaleMode ScaleMode { get; set; }
        [DataMember]
        public EProductDescriptionState State { get; set; }
        [DataMember]
        public WeightInfo Weight { get; set; }
        [DataMember]
        public int WeightClass { get; set; }
        [DataMember]
        public List<AttributeInfo> Attributes { get; set; }
        [DataMember]
        public List<string> ReadyModeSelection { get; set; }
        [DataMember]
        public int ProductCode { get; set; }
        [DataMember]
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

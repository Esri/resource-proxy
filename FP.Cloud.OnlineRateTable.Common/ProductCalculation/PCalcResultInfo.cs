using System;
using System.Runtime.Serialization;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    [DataContract]
    public class PCalcResultInfo
    {
        #region properties
        [DataMember]
        public ProductDescriptionInfo ProductDescription { get; set; }
        [DataMember]
        public TransferDescriptionInfo QueryDescription { get; set; }
        [DataMember]
        public EQueryType QueryType { get; set; }
        [IgnoreDataMember]
        public DescriptionInfo DedicatedDescription
        {
            get { return GetDedicatedDescription(); }
        }
        #endregion

        #region private
        private DescriptionInfo GetDedicatedDescription()
        {
            switch (QueryType)
            {
                case EQueryType.RequestPostage:
                case EQueryType.RequestValue:
                case EQueryType.RequestString:
                    return new RequestDescriptionInfo(QueryDescription);
                case EQueryType.SelectIndex:
                    return new SelectIndexDescriptionInfo(QueryDescription);
                case EQueryType.ShowMenu:
                    return new ShowMenuDescriptionInfo(QueryDescription);
                case EQueryType.SelectValue:
                    return new SelectValueDescriptionInfo(QueryDescription);
                case EQueryType.ShowDisplay:
                default:
                    return new DescriptionInfo(QueryDescription);
            }
        }
        #endregion
    }
}

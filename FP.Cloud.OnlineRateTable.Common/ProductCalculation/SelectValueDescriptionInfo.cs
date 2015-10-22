using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    [DataContract]
    public class SelectValueDescriptionInfo : DescriptionInfo
    {
        #region properties
        [DataMember]
        public List<ValueEntryInfo> ValueEntries { get; set; }
        #endregion

        #region constructor
        public SelectValueDescriptionInfo()
        {
            ValueEntries = new List<ValueEntryInfo>();
        }
        #endregion
    }
}

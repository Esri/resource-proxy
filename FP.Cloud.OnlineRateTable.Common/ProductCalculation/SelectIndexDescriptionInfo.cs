using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    [DataContract]
    public class SelectIndexDescriptionInfo : DescriptionInfo
    {
        #region properties
        [DataMember]
        public List<string> IndexEntries { get; set; }
        #endregion

        #region constructor
        public SelectIndexDescriptionInfo()
        {
            IndexEntries = new List<string>();
        }
        #endregion
    }
}

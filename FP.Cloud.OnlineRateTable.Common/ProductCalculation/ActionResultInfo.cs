using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    [DataContract]
    public class ActionResultInfo
    {
        #region properties 
        [DataMember]
        public EActionId Action { get; set; }
        [DataMember]
        public int Label { get; set; }
        [DataMember]
        public List<AnyInfo> Results { get; set; }
        #endregion

        #region constructor
        public ActionResultInfo()
        {
            Results = new List<AnyInfo>();
        }
        #endregion
    }
}

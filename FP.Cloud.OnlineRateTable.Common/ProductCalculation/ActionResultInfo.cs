using System.Collections.Generic;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    public class ActionResultInfo
    {
        #region properties  
        public EActionId Action { get; set; }
        public int Label { get; set; }
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

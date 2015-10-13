using System.Collections.Generic;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    public class ActionResultInfo
    {   
        public EActionId Action { get; set; }
        public int Label { get; set; }
        public IEnumerable<AnyInfo> Results { get; set; }
    }
}

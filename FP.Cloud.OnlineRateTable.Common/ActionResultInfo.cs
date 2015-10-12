using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP.Cloud.OnlineRateTable.Common
{
    public class ActionResultInfo
    {   
        public EActionId Action { get; set; }
        public int Label { get; set; }
        public IEnumerable<AnyInfo> Results { get; set; }
    }
}

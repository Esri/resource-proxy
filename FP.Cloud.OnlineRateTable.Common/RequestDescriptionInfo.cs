using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP.Cloud.OnlineRateTable.Common
{
    public class RequestDescriptionInfo : DescriptionInfo
    {
        public string StatusMessage { get; set; }
        public int Label { get; set; }
        public string DisplayFormat { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP.Cloud.OnlineRateTable.Common
{
    public class PostageInfo
    {
        public UInt64 PostageValue { get; set; }
        public ushort PostageDecimals { get; set; }
        public string CurrencySymbol { get; set; }
    }
}

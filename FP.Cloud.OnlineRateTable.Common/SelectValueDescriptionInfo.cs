using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP.Cloud.OnlineRateTable.Common
{
    public class SelectValueDescriptionInfo : DescriptionInfo
    {
        IEnumerable<ValueEntryInfo> ValueEntries { get; set; }
    }
}

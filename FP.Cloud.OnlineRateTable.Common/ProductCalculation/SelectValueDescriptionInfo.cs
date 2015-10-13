using System.Collections.Generic;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    public class SelectValueDescriptionInfo : DescriptionInfo
    {
        IEnumerable<ValueEntryInfo> ValueEntries { get; set; }
    }
}

using System.Collections.Generic;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    public class ShowMenuDescriptionInfo : DescriptionInfo
    {
        IEnumerable<string> MenuEntries { get; set; }
        public string AdditionalInfo { get; set; }
    }
}

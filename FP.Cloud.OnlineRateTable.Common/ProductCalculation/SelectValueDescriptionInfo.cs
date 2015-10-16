using System.Collections.Generic;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    public class SelectValueDescriptionInfo : DescriptionInfo
    {
        #region properties
        List<ValueEntryInfo> ValueEntries { get; set; }
        #endregion

        #region constructor
        public SelectValueDescriptionInfo()
        {
            ValueEntries = new List<ValueEntryInfo>();
        }
        #endregion
    }
}

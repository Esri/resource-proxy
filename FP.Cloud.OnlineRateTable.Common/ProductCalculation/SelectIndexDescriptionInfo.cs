using System.Collections.Generic;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    public class SelectIndexDescriptionInfo : DescriptionInfo
    {
        #region properties
        public List<string> IndexEntries { get; set; }
        #endregion

        #region constructor
        public SelectIndexDescriptionInfo()
        {
            IndexEntries = new List<string>();
        }
        #endregion
    }
}

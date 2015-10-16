using System.Collections.Generic;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    public class ShowMenuDescriptionInfo : DescriptionInfo
    {
        #region properties
        List<string> MenuEntries { get; set; }
        public string AdditionalInfo { get; set; }
        #endregion

        #region constructor
        public ShowMenuDescriptionInfo()
        {
            MenuEntries = new List<string>();
        }
        #endregion
    }
}

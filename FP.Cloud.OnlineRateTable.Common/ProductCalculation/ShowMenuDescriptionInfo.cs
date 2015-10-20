using System;
using System.Collections.Generic;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    public class ShowMenuDescriptionInfo : DescriptionInfo
    {
        #region Constructors and Destructors

        #region constructor

        public ShowMenuDescriptionInfo()
        {
            MenuEntries = new List<string>();
        }

        #endregion

        #endregion

        #region properties

        public List<string> MenuEntries { get; set; }

        public string AdditionalInfo { get; set; }

        #endregion
    }
}
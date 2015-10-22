using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    [DataContract]
    public class ShowMenuDescriptionInfo : DescriptionInfo
    {
        #region properties
        [DataMember]
        public List<string> MenuEntries { get; set; }
        [DataMember]
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
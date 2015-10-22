using FP.Cloud.OnlineRateTable.Common.ProductCalculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    [DataContract]
    public class TransferDescriptionInfo
    {
        #region properties
        [DataMember]
        public string DescriptionTitle { get; set; }

        [DataMember]
        public List<ValueEntryInfo> ValueEntries { get; set; }

        [DataMember]
        public List<string> Entries { get; set; }

        [DataMember]
        public string StatusMessage { get; set; }

        [DataMember]
        public int Label { get; set; }

        [DataMember]
        public string DisplayFormat { get; set; }

        [DataMember]
        public string AdditionalInfo { get; set; }
        #endregion

        #region constructor
        public TransferDescriptionInfo()
        {
            ValueEntries = new List<ValueEntryInfo>();
            Entries = new List<string>();
        }
        #endregion
    }
}

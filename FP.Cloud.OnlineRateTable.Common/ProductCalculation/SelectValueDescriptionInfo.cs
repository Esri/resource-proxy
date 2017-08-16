using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    public class SelectValueDescriptionInfo : DescriptionInfo
    {
        #region properties
        public List<ValueEntryInfo> ValueEntries { get; set; }
        #endregion

        #region constructor
        public SelectValueDescriptionInfo()
        {
            ValueEntries = new List<ValueEntryInfo>();
        }
        
        public SelectValueDescriptionInfo(TransferDescriptionInfo transfer) : base(transfer)
        {
            ValueEntries = transfer.ValueEntries;
        }
        #endregion

        #region public
        public override TransferDescriptionInfo ToTransferDescription()
        {
            TransferDescriptionInfo transfer = base.ToTransferDescription();
            transfer.ValueEntries = ValueEntries;
            return transfer;
        }
        #endregion
    }
}

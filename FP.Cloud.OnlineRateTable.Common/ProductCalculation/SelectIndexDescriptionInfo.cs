using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
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

        public SelectIndexDescriptionInfo(TransferDescriptionInfo transfer) : base(transfer)
        {
            IndexEntries = transfer.Entries;
        }
        #endregion

        #region public
        public override TransferDescriptionInfo ToTransferDescription()
        {
            TransferDescriptionInfo transfer = base.ToTransferDescription();
            transfer.Entries = IndexEntries;
            return transfer;
        }
        #endregion
    }
}

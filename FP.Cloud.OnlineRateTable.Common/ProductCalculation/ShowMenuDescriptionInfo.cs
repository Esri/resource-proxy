using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    public class ShowMenuDescriptionInfo : DescriptionInfo
    {
        #region properties
        public List<string> MenuEntries { get; set; }
        public string AdditionalInfo { get; set; }
        #endregion

        #region constructor
        public ShowMenuDescriptionInfo()
        {
            MenuEntries = new List<string>();
        }

        public ShowMenuDescriptionInfo(TransferDescriptionInfo transfer) : base(transfer)
        {
            MenuEntries = transfer.Entries;
            AdditionalInfo = transfer.AdditionalInfo;
        }
        #endregion

        #region public
        public override TransferDescriptionInfo ToTransferDescription()
        {
            TransferDescriptionInfo transfer = base.ToTransferDescription();
            transfer.Entries = MenuEntries;
            transfer.AdditionalInfo = AdditionalInfo;
            return transfer;
        }
        #endregion
    }
}
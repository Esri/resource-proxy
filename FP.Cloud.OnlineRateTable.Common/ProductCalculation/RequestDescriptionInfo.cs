using System;
using System.Runtime.Serialization;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    public class RequestDescriptionInfo : DescriptionInfo
    {
        #region properties
        public string StatusMessage { get; set; }
        public int Label { get; set; }
        public string DisplayFormat { get; set; }
        #endregion

        #region constructor
        public RequestDescriptionInfo() { }

        public RequestDescriptionInfo(TransferDescriptionInfo transfer) : base(transfer)
        {
            StatusMessage = transfer.StatusMessage;
            Label = transfer.Label;
            DisplayFormat = transfer.DisplayFormat;
        }
        #endregion

        #region public
        public override TransferDescriptionInfo ToTransferDescription()
        {
            TransferDescriptionInfo transfer = base.ToTransferDescription();
            transfer.StatusMessage = StatusMessage;
            transfer.Label = Label;
            transfer.DisplayFormat = DisplayFormat;
            return transfer;
        }
        #endregion
    }
}

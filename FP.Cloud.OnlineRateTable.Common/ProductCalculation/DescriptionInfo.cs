using System;
using System.Runtime.Serialization;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    public class DescriptionInfo
    {
        #region properties
        public string DescriptionTitle { get; set; }
        #endregion

        #region constructor
        public DescriptionInfo() { }

        public DescriptionInfo(TransferDescriptionInfo transfer)
        {
            DescriptionTitle = transfer.DescriptionTitle;
        }
        #endregion

        #region public
        public virtual TransferDescriptionInfo ToTransferDescription()
        {
            TransferDescriptionInfo transfer = new TransferDescriptionInfo();
            transfer.DescriptionTitle = DescriptionTitle;
            return transfer;
        }
        #endregion
    }
}

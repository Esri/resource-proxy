using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP.Cloud.OnlineRateTable.Common.RateTable
{
    public class RateTableFileInfo
    {
        #region properties
        public int Id { get; set; }
        public string FileName { get; set; }
        public EFileType FileType { get; set; }
        public byte[] FileData { get; set; }
        #endregion

        #region constructor
        public RateTableFileInfo()
        {
            FileData = new byte[0];
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP.Cloud.OnlineRateTable.Common
{
    public class ProductDescriptionInfo
    {
        public PostageInfo Postage { get; set; }
        public int ProductId { get; set; }
        public EScaleMode ScaleMode { get; set; }
        public EProductDescriptionState State { get; set; }
        public WeightInfo Weight { get; set; }
        public int WeightClass { get; set; }
        IEnumerable<AttributeInfo> Attribute { get; set; }
        IEnumerable<string> ReadyModeSelection { get; set; }
        public int ProductCode { get; set; }
        public int RateVersion { get; set; }
    }
}

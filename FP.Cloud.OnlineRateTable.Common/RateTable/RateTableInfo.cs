using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP.Cloud.OnlineRateTable.Common.RateTable
{
    public class RateTableInfo
    {
        public int Id { get; set; }
        public string Variant { get; set; }
        public string VersionNumber { get; set; }
        public int CarrierId { get; set; }
        public int CarrierDetails { get; set; }
        public DateTime ValidFrom { get; set; }
        public string Culture { get; set; }
        public IEnumerable<RateTableFileInfo> PackageFiles { get; set; }
    }
}

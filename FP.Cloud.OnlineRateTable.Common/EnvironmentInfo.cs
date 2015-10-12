using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP.Cloud.OnlineRateTable.Common
{
    public class EnvironmentInfo
    {
        public string Iso3CountryCode { get; set; }
        public int CarrierId { get; set; }
        public DateTime UtcDate { get; set; }
        public string Culture { get; set; }
    }
}

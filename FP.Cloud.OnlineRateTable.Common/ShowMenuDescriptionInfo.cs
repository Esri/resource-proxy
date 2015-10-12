using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP.Cloud.OnlineRateTable.Common
{
    public class ShowMenuDescriptionInfo : DescriptionInfo
    {
        IEnumerable<string> MenuEntries { get; set; }
        public string AdditionalInfo { get; set; }
    }
}

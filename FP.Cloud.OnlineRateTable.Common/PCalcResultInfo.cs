using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP.Cloud.OnlineRateTable.Common
{
    public class PCalcResultInfo
    {
        public ProductDescriptionInfo ProductDescription { get; set; }
        DescriptionInfo QueryDescription { get; set; }
        EQueryType QueryType { get; set; }
    }
}

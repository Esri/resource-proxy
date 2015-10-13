using System.Collections.Generic;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    public class AttributeInfo
    {
        public int Key { get; set; }
        public IEnumerable<int> Values { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP.Cloud.OnlineRateTable.Common
{
    public class AttributeInfo
    {
        public int Key { get; set; }
        public IEnumerable<int> Values { get; set; }
    }
}

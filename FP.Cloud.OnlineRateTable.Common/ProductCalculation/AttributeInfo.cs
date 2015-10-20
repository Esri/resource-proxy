using System;
using System.Collections.Generic;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    public class AttributeInfo
    {
        #region properties
        public int Key { get; set; }
        public List<int> Values { get; set; }
        #endregion

        #region constructor
        public AttributeInfo()
        {
            Values = new List<int>();
        }
        #endregion
    }
}

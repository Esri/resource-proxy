using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    [DataContract]
    public class AttributeInfo
    {
        #region properties
        [DataMember]
        public int Key { get; set; }
        [DataMember]
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

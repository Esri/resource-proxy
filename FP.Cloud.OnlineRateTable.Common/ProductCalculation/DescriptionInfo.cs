using System;
using System.Runtime.Serialization;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    [DataContract]
    public class DescriptionInfo
    {
        [DataMember]
        public string DescriptionTitle { get; set; }
        [DataMember]
        public Type Type
        {
            get { return GetType(); }
        }
    }
}

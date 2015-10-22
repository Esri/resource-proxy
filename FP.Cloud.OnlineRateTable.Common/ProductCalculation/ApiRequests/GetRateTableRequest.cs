using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation.ApiRequests
{
    [Serializable]
    [DataContract]
    public class GetRateTableRequest
    {
        [DataMember]
        public string Variant { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public int? Carrier { get; set; }
        [DataMember]
        public DateTime ValidFrom { get; set; }
        [DataMember]
        public string Culture { get; set; }
        [DataMember]
        public int StartValue { get; set; }
        [DataMember]
        public int ItemCount { get; set; }
        [DataMember]
        public bool IncludeFileData{ get; set; }
    }
}
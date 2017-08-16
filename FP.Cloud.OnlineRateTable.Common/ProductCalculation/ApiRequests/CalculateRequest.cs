using FP.Cloud.OnlineRateTable.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FP.Cloud.OnlineRateTable.Common.ProductCalculation;
using System.Runtime.Serialization;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation.ApiRequests
{
    [Serializable]
    [DataContract]
    public class CalculateRequest : CalculationBaseRequest
    {
        [DataMember]
        public ProductDescriptionInfo ProductDescription { get; set; }
        [DataMember]
        public ActionResultInfo ActionResult { get; set; }
    }
}
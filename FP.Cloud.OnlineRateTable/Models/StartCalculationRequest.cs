using FP.Cloud.OnlineRateTable.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FP.Cloud.OnlineRateTable.Models
{
    public class StartCalculationRequest : BaseRequest
    {
        public WeightInfo Weight { get; set; }
    }
}
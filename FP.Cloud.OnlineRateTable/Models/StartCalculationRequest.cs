﻿using FP.Cloud.OnlineRateTable.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FP.Cloud.OnlineRateTable.Common.ProductCalculation;

namespace FP.Cloud.OnlineRateTable.Models
{
    public class StartCalculationRequest : BaseRequest
    {
        public WeightInfo Weight { get; set; }
    }
}
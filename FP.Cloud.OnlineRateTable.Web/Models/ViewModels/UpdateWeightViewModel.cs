using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FP.Cloud.OnlineRateTable.Web.Models.ViewModels
{
    public class UpdateWeightViewModel
    {
        public decimal WeightValue { get; set; }
        public bool CultureIsMetric { get; set; }
    }
}
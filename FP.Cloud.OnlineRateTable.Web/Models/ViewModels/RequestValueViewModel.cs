using FP.Cloud.OnlineRateTable.Common.ProductCalculation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;

namespace FP.Cloud.OnlineRateTable.Web.Models.ViewModels
{
    public class RequestValueViewModel
    {
        #region properties
        public int Label { get; set; }
        public string FormatString { get; set; }
        [Required(ErrorMessage ="Entering a value is required")]
        [DisplayName("Value")]
        public string EnteredRawValue { get; set; }
        #endregion
    }
}
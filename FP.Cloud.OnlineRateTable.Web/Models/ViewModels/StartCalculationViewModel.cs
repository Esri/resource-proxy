using FP.Cloud.OnlineRateTable.Common.RateTable;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FP.Cloud.OnlineRateTable.Web.Models.ViewModels
{
    public class StartCalculationViewModel : ProductCalculationViewModel
    {
        [DisplayName("Select Country")]
        [Required(ErrorMessage ="Selecting a country is required")]
        public string SelectedRateTable { get; set; }

        [DisplayName("Sender Zip Code")]
        [Required(ErrorMessage ="Entering a sender zip code is required")]
        public string SenderZip { get; set; }
    }
}
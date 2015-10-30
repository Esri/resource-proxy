using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FP.Cloud.OnlineRateTable.Web.Models.ViewModels
{
    public class UpdateWeightViewModel
    {
        [Required(ErrorMessage = "Please enter a valid weight in gram")]
        [Display(Name ="Weight")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:#0.0}")]
        [Range(0.0, 70000.0, ErrorMessage ="Please enter a weight between {1}g and {2}g")]
        public decimal WeightValueInGram { get; set; }

        [Required(ErrorMessage = "Please enter a valid weight in ounces")]
        [Display(Name = "Weight")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:#0.0}")]
        [Range(0.0, 1120.0, ErrorMessage = "Please enter a weight between {1}oz and {2}oz")]
        public decimal WeightValueInOunces { get; set; }

        public decimal WeightValue
        {
            get { return CultureIsMetric ? WeightValueInGram : WeightValueInOunces; }
        }
        public bool CultureIsMetric { get; set; }
    }
}
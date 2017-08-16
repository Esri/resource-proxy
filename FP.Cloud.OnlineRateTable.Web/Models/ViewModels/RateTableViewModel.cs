using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FP.Cloud.OnlineRateTable.Web.Models.ViewModels
{
    public class RateTableViewModel
    {
        [DataType(DataType.Date, ErrorMessage = "This is not a valid date")]
        [Display(Name = "Valid From")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please specify a validity start date for the rate table")]
        public DateTime ValidFrom { get; set; }

        [Display(Name = "Language")]
        [Required(ErrorMessage = "Please specify a valid language description")]
        public string Culture { get; set; }

        //[DataType(DataType.Upload)]
        //[Required(ErrorMessage ="Please specify a valid file upload path")]
        //public HttpPostedFileBase ZipUpload { get; set; }
    }
}
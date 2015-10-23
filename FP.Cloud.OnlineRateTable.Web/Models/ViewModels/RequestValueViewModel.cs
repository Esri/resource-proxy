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
        public string EnteredStringValue { get; set; }

        //[Range(typeof(decimal), "0", "10000", ErrorMessage = "{0} must be a decimal number between {1} and {2}")]
        [Required(ErrorMessage = "{0} is required.")]
        [DataType(DataType.Currency, ErrorMessage = "This is not a valid postage amount")]
        [DisplayName("Postage")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}", ConvertEmptyStringToNull = true)]
        public decimal EnteredPostageValue { get; set; }
        #endregion

        #region public
        public string GetValueAsString(CultureInfo culture, EQueryType queryType)
        {
            switch (queryType)
            {
                case EQueryType.RequestPostage:
                    return EnteredPostageValue.ToString(culture);
                case EQueryType.RequestValue:
                    return EnteredStringValue;
                case EQueryType.RequestString:
                    return EnteredStringValue;
            }
            return string.Empty;
        }
        #endregion
    }
}
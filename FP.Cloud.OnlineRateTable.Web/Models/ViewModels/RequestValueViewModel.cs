using FP.Cloud.OnlineRateTable.Common.ProductCalculation;
using System;
using System.Collections.Generic;
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
        public EQueryType QueryType { get; set; }
        public string FormatString { get; set; }

        public string EnteredStringValue { get; set; }

        [DataType(DataType.Currency, ErrorMessage ="This is not a valid postage amount")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:c}", ConvertEmptyStringToNull = true)]
        public decimal EnteredPostageValue { get; set; }
        #endregion

        #region public
        public string GetValueAsString(CultureInfo culture)
        {
            switch (QueryType)
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
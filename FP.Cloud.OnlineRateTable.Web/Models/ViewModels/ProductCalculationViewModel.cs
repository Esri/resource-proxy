using FP.Cloud.OnlineRateTable.Common.ProductCalculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FP.Cloud.OnlineRateTable.Web.Models.ViewModels
{
    public class ProductCalculationViewModel
    {
        #region properties
        public RequestValueViewModel RequestValueModel {get; set;}
        public EQueryType QueryType { get; set; }
        public string PartialView
        {
            get { return GetPartialView(); }
        }
        #endregion

        #region constructor
        public ProductCalculationViewModel(EQueryType type)
        {
            RequestValueModel = new RequestValueViewModel();
            QueryType = type;
        }

        public ProductCalculationViewModel()
        {
            RequestValueModel = new RequestValueViewModel();
        }
        #endregion

        #region private
        private string GetPartialView()
        {
            switch (QueryType)
            {
                case EQueryType.RequestPostage:
                case EQueryType.RequestValue:
                case EQueryType.RequestString:
                    return "~/Views/ProductCalculation/_RequestValuePartial.cshtml";
                case EQueryType.SelectIndex:
                    return "~/Views/ProductCalculation/_SelectIndexPartial.cshtml";
                    break;
                case EQueryType.ShowMenu:
                    return "~/Views/ProductCalculation/_ShowMenuPartial.cshtml";
                case EQueryType.ShowDisplay:
                    return "~/Views/ProductCalculation/_ShowDisplayPartial.cshtml";
                case EQueryType.SelectValue:
                    return "~/Views/ProductCalculation/_SelectValuePartial.cshtml";
                default:
                    return string.Empty;
            }
        }
        #endregion
    }
}
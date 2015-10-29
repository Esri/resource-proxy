﻿using FP.Cloud.OnlineRateTable.Common.ProductCalculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FP.Cloud.OnlineRateTable.Web.Models.ViewModels
{
    public class ProductCalculationViewModel
    {
        #region properties
        public UpdateWeightViewModel UpdateWeightModel { get; set; }
        public EQueryType QueryType { get; set; }
        public string PartialView
        {
            get { return GetPartialView(); }
        }
        #endregion

        #region constructor
        protected ProductCalculationViewModel()
        {
            UpdateWeightModel = new UpdateWeightViewModel();
        }
        #endregion

        #region public
        public static ProductCalculationViewModel Create(EQueryType type)
        {
            switch (type)
            {
                case EQueryType.None:
                    return new StartCalculationViewModel() { QueryType = type } ;
                case EQueryType.RequestPostage:
                case EQueryType.RequestValue:
                case EQueryType.RequestString:
                    return new RequestValueViewModel() { QueryType = type };
                case EQueryType.SelectIndex:
                case EQueryType.ShowMenu:
                case EQueryType.ShowDisplay:
                case EQueryType.SelectValue:
                default:
                    return new ProductCalculationViewModel() { QueryType = type };
            }
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
using FP.Cloud.OnlineRateTable.Common.ProductCalculation;
using FP.Cloud.OnlineRateTable.Common.ProductCalculation.ApiRequests;
using FP.Cloud.OnlineRateTable.Common.RateTable;
using FP.Cloud.OnlineRateTable.Web.CustomAttributes;
using FP.Cloud.OnlineRateTable.Web.Formatter;
using FP.Cloud.OnlineRateTable.Web.Models.ViewModels;
using FP.Cloud.OnlineRateTable.Web.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FP.Cloud.OnlineRateTable.Web.Controllers
{
    [CustomErrorHandler(View = "Error")]
    public class ProductCalculationController : BaseController
    {
        #region const
        private const string PCALC_RESULT = "PcalcResult";
        public static readonly string PRODUCT_DESCRIPTION = "ProductDescription";
        public static readonly string REQUEST_DESCRIPTION = "RequestDescription";
        public static readonly string POSTAGE = "Postage";
        public static readonly string WEIGHT = "Weight";
        public static readonly string ACTIVE_RATE_TABLES = "ActiveRateTables";
        public static readonly string ENVIRONMENT = "Environment";
        #endregion

        #region members
        private ProductCalculationRepository m_Repository;
        #endregion

        #region constructor
        public ProductCalculationController(ProductCalculationRepository repository)
        {
            m_Repository = repository;
        }
        #endregion
        
        // GET: ProductCalculation
        public async Task<ActionResult> Index()
        {
            ApiResponse<List<RateTableInfo>> allActive = await m_Repository.GetActiveRateTables(DateTime.Now);
            if(IsApiError(allActive) || null == allActive.ApiResult || allActive.ApiResult.Count == 0)
            {
                return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Unable to retrieve active rate tables");
            }
            ViewData.Add(ACTIVE_RATE_TABLES, allActive.ApiResult);
            return View(ProductCalculationViewModel.Create(EQueryType.None));
        }

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Start(StartCalculationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Unable to create environment");
            }
            int rateTableId = 0;
            int.TryParse(model.SelectedRateTable, out rateTableId);
            ApiResponse<EnvironmentInfo> environmentResponse = await m_Repository.CreateEnvironment(rateTableId);
            if(IsApiError(environmentResponse) || environmentResponse.ApiResult == null)
            {
                return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Unable to create environment");
            }

            SetUiCulture(environmentResponse.ApiResult.Culture);
            StartCalculationRequest request = new StartCalculationRequest();
            request.Weight = GetInitialWeight(environmentResponse.ApiResult.Culture);
            environmentResponse.ApiResult.SenderZipCode = model.SenderZip;
            request.Environment = environmentResponse.ApiResult;

            ApiResponse<PCalcResultInfo> startResponse = await m_Repository.Start(request);
            if(IsApiError(startResponse) || PcalcResultIsValid(startResponse.ApiResult) == false)
            {
                return HandleGeneralPcalcError("Index", ProductCalculationViewModel.Create(EQueryType.None), startResponse.ApiResult);
            }

            AddOrUpdateTempData(startResponse.ApiResult, request.Environment);
            AddViewData(startResponse.ApiResult, environmentResponse.ApiResult);
            return View("Index", ProductCalculationViewModel.Create(startResponse.ApiResult.QueryType));
        }

        public async Task<ActionResult> Restart()
        {
            StartCalculationRequest request = new StartCalculationRequest();
            PCalcResultInfo lastInfo = GetLastPcalcResult();
            //try to re-use the weight
            request.Weight = null != lastInfo?.ProductDescription ? lastInfo?.ProductDescription.Weight : 
                new WeightInfo() { WeightUnit = EWeightUnit.Gram, WeightValue = 0 };

            EnvironmentInfo environment = GetEnvironment();
            if (null == environment)
            {
                return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Unable to create environment");
            }

            request.Environment = environment;
            ApiResponse<PCalcResultInfo> restartResult = await m_Repository.Start(request);
            if(IsApiError(restartResult) || PcalcResultIsValid(restartResult.ApiResult) == false)
            {
                return HandleGeneralPcalcError("Index", ProductCalculationViewModel.Create(EQueryType.None), restartResult.ApiResult);
            }

            AddOrUpdateTempData(restartResult.ApiResult, request.Environment);
            AddViewData(restartResult.ApiResult, environment);
            return View("Index", ProductCalculationViewModel.Create(restartResult.ApiResult.QueryType));

        }

        public async Task<ActionResult> StepBack()
        {
            PCalcResultInfo lastResult = GetLastPcalcResult();
            if (null == lastResult)
            {
                return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Unable to retrieve last result");
            }
            UpdateRequest request = new UpdateRequest();
            EnvironmentInfo environment = GetEnvironment();
            if (null == environment)
            {
                return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Unable to create environment");
            }
            request.Environment = environment;
            request.ProductDescription = lastResult.ProductDescription;
            ApiResponse<PCalcResultInfo> response = await m_Repository.StepBack(request);
            if (IsApiError(response) || PcalcResultIsValid(response.ApiResult) == false)
            {
                return HandleGeneralPcalcError("Index", ProductCalculationViewModel.Create(EQueryType.None), response.ApiResult);
            }

            AddOrUpdateTempData(response.ApiResult, request.Environment);
            AddViewData(response.ApiResult, environment);
            return View("Index", ProductCalculationViewModel.Create(response.ApiResult.QueryType));
        }

        public async Task<ActionResult> Finish()
        {
            PCalcResultInfo lastResult = GetLastPcalcResult();
            if (null == lastResult)
            {
                return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Unable to retrieve last result");
            }
            EnvironmentInfo environment = GetEnvironment();
            if (null == environment)
            {
                return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Unable to create environment");
            }

            //stupid read once temp data...
            AddOrUpdateTempData(lastResult, environment);
            AddViewData(lastResult, environment);
            ProductCalculationViewModel model = ProductCalculationViewModel.Create(lastResult.QueryType);
            model.ProductCalculationFinished = true;
            return View("Index", model);
        }

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateWeight([Bind(Include = "WeightValueInGram,WeightValueInOunces,CultureIsMetric,ProductCalculationFinished")]UpdateWeightViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Unspecified error");
            }
            PCalcResultInfo lastResult = GetLastPcalcResult();
            if (null == lastResult)
            {
                return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Unable to retrieve last result");
            }
            UpdateRequest request = new UpdateRequest();
            EnvironmentInfo environment = GetEnvironment();
            if (null == environment)
            {
                return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Unable to create environment");
            }
            request.Environment = environment;
            request.ProductDescription = lastResult.ProductDescription;
            int weightValue = (int)(model.WeightValue * 10);
            request.ProductDescription.Weight = new WeightInfo()
            {
                WeightValue = weightValue,
                WeightUnit = model.CultureIsMetric ? EWeightUnit.TenthGram : EWeightUnit.TenthOunce
            };

            ApiResponse<PCalcResultInfo> response = await m_Repository.UpdateWeight(request);
            if(IsApiError(response))
            {
                return HandleGeneralPcalcError("Index", ProductCalculationViewModel.Create(EQueryType.None), response.ApiResult);
            }

            //update weight will only update the product description if the weight is valid
            response.ApiResult.QueryDescription = response.ApiResult.QueryDescription == null ? 
                lastResult.QueryDescription : response.ApiResult.QueryDescription;
            response.ApiResult.QueryType = response.ApiResult.QueryType == EQueryType.None ? 
                lastResult.QueryType : response.ApiResult.QueryType;
            if (PcalcResultIsValid(response.ApiResult) == false)
            {
                return HandleGeneralPcalcError("Index", ProductCalculationViewModel.Create(EQueryType.None), response.ApiResult);
            }
            AddOrUpdateTempData(response.ApiResult, request.Environment);
            AddViewData(response.ApiResult, environment);
            ProductCalculationViewModel newModel = ProductCalculationViewModel.Create(response.ApiResult.QueryType);
            newModel.ProductCalculationFinished = model.ProductCalculationFinished;
            return View("Index", newModel);            
        }

        public async Task<ActionResult> SelectMenuIndex(int index)
        {
            var actionResult = new ActionResultInfo()
            {
                Action = EActionId.ShowMenu,
                Results = new List<AnyInfo>()
                {
                    new AnyInfo() { AnyType = EAnyType.INT32, AnyValue=index.ToString()}
                }
            };
            return await HandleCalculation(actionResult, GetEnvironment());
        }

        public async Task<ActionResult> SelectIndex(int index)
        {
            var actionResult = new ActionResultInfo()
            {
                Action = EActionId.SelectIndex,
                Results = new List<AnyInfo>()
                {
                    new AnyInfo() { AnyType = EAnyType.INT32, AnyValue=index.ToString()}
                }
            };
            return await HandleCalculation(actionResult, GetEnvironment());
        }

        public async Task<ActionResult> SelectValue(int entryValue)
        {
            var actionResult = new ActionResultInfo()
            {
                Action = EActionId.SelectValue,
                Results = new List<AnyInfo>()
                {
                    new AnyInfo() { AnyType = EAnyType.INT32, AnyValue=entryValue.ToString()}
                }
            };
            return await HandleCalculation(actionResult, GetEnvironment());
        }

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RequestValue(RequestValueViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Product Calculation Error");
            }
            EnvironmentInfo environment = GetEnvironment();
            if (null == environment)
            {
                return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Unable to retrieve environment information");
            }
            CultureInfo culture = new CultureInfo(environment.Culture);
            char decimalSeperator = culture.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0];

            FormatStringAdapter adapter = new FormatStringAdapter(decimalSeperator);
            adapter.SetFormatString(model.FormatString);

            string formattedString;
            adapter.Format(out formattedString, model.EnteredRawValue);

            var actionResult = new ActionResultInfo()
            {
                Action = model.QueryType == EQueryType.RequestValue ? EActionId.RequestValue :
                model.QueryType == EQueryType.RequestPostage ? EActionId.ManualPostage : EActionId.RequestString,
                Results = new List<AnyInfo>()
            };

            uint numberOfEntries = adapter.GetValueNumber();
            for (uint i = 0; i < numberOfEntries; ++i)
            {
                object valuePart = adapter.GetValue(formattedString, i);
                if (valuePart != null)
                {
                    actionResult.Results.Add(new AnyInfo()
                    {
                        AnyType = model.QueryType == EQueryType.RequestString ? EAnyType.STRING : EAnyType.UINT32,
                        AnyValue = valuePart.ToString()
                    });
                }
            }
            return await HandleCalculation(actionResult, environment);
        }

        public async Task<ActionResult> Acknowledge()
        {
            var actionResult = new ActionResultInfo()
            {
                Action = EActionId.Display,
                Results = new List<AnyInfo>()
                {
                    new AnyInfo() { AnyType = EAnyType.INT32, AnyValue = ((int)EActionDisplayResult.DISPLAYED).ToString()}
                }
            };
            return await HandleCalculation(actionResult, GetEnvironment());
        }

        [HttpPost]
        public ActionResult FormatInput(string formatString, string inputString, string cultureString)
        {
            CultureInfo culture = new CultureInfo(cultureString);
            char decimalSeperator = culture.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0];

            FormatStringAdapter adapter = new FormatStringAdapter(decimalSeperator);
            adapter.SetFormatString(formatString);

            string formattedString;
            adapter.Format(out formattedString, inputString);

            return Json(new { Success = !string.IsNullOrEmpty(formattedString), DisplayString = formattedString });
        }

        #region private
        private async Task<ActionResult> HandleCalculation(ActionResultInfo actionResult, EnvironmentInfo environment)
        {
            PCalcResultInfo lastResult = GetLastPcalcResult();
            if (null == lastResult)
            {
                return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Unable to retrieve last product");
            }
            if (null == environment)
            {
                return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Unable to retrieve environment information");
            }

            CalculateRequest calc = new CalculateRequest();
            calc.Environment = environment;
            calc.ProductDescription = lastResult.ProductDescription;
            calc.ActionResult = actionResult;
            ApiResponse<PCalcResultInfo> response = await m_Repository.Calculate(calc);
            if (IsApiError(response) || PcalcResultIsValid(response.ApiResult) == false)
            {
                return HandleGeneralPcalcError("Index", ProductCalculationViewModel.Create(EQueryType.None), response.ApiResult);
            }
            AddOrUpdateTempData(response.ApiResult, calc.Environment);
            AddViewData(response.ApiResult, environment);
            return View("Index", ProductCalculationViewModel.Create(response.ApiResult.QueryType));            
        }

        private void AddOrUpdateTempData(PCalcResultInfo result, EnvironmentInfo environment)
        {
            if (TempData.ContainsKey(PCALC_RESULT))
            {
                TempData[PCALC_RESULT] = result;
            }
            else
            {
                TempData.Add(PCALC_RESULT, result);
            }
            if (TempData.ContainsKey(ENVIRONMENT))
            {
                TempData[ENVIRONMENT] = environment;
            }
            else
            {
                TempData.Add(ENVIRONMENT, environment);
            }
        }

        private PCalcResultInfo GetLastPcalcResult()
        {
            if(TempData.Keys.Contains(PCALC_RESULT))
            {
                return (PCalcResultInfo)TempData[PCALC_RESULT];
            }
            return null;
        }

        private EnvironmentInfo GetEnvironment()
        {
            if (TempData.Keys.Contains(ENVIRONMENT))
            {
                return (EnvironmentInfo)TempData[ENVIRONMENT];
            }
            return null;
        }

        private WeightInfo GetInitialWeight(string culture)
        {
            CultureInfo cultureInfo = new CultureInfo(culture);
            RegionInfo region = new RegionInfo(cultureInfo.LCID);
            EWeightUnit unit = region.IsMetric ? EWeightUnit.Gram : EWeightUnit.TenthOunce;
            return new WeightInfo() { WeightUnit = unit, WeightValue = 1 };
        }

        private void SetUiCulture(string culture)
        {
            Session["Language"] = culture;
        }

        private void AddViewData(PCalcResultInfo result, EnvironmentInfo environment)
        {
            ViewData.Add(PRODUCT_DESCRIPTION, result?.ProductDescription);
            ViewData.Add(WEIGHT, result?.ProductDescription?.Weight);
            ViewData.Add(POSTAGE, result?.ProductDescription?.Postage);
            ViewData.Add(REQUEST_DESCRIPTION, result?.DedicatedDescription);
            ViewData.Add(ENVIRONMENT, environment);
        }

        private bool PcalcResultIsValid(PCalcResultInfo info)
        {
            return null != info && 
                null != info.QueryDescription && 
                null != info.ProductDescription;
        }
        #endregion

        #region protected
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_Repository.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
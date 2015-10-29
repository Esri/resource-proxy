using FP.Cloud.OnlineRateTable.Common.ProductCalculation;
using FP.Cloud.OnlineRateTable.Common.ProductCalculation.ApiRequests;
using FP.Cloud.OnlineRateTable.Common.RateTable;
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
            IEnumerable<RateTableInfo> allActive = await m_Repository.GetActiveRateTables(DateTime.Now);
            if (null != allActive && allActive.Count() > 0)
            {
                ViewData.Add(ACTIVE_RATE_TABLES, allActive);
                return View(ProductCalculationViewModel.Create(EQueryType.None));
            }
            return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Unable to retrieve active rate tables");
        }

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Start(StartCalculationViewModel model)
        {
            if (ModelState.IsValid)
            {
                int rateTableId = 0;
                int.TryParse(model.SelectedRateTable, out rateTableId);
                EnvironmentInfo environment = await m_Repository.CreateEnvironment(rateTableId);
                if (null != environment)
                {
                    StartCalculationRequest request = new StartCalculationRequest();
                    request.Weight = new WeightInfo() { WeightUnit = EWeightUnit.Gram, WeightValue = 0 };
                    environment.SenderZipCode = model.SenderZip;
                    request.Environment = environment;
                    PCalcResultInfo result = await m_Repository.Start(request);
                    if (PcalcResultIsValid(result))
                    {
                        AddOrUpdateTempData(result, request.Environment);
                        AddViewData(result, environment);
                        return View("Index", ProductCalculationViewModel.Create(result.QueryType));
                    }
                    return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), result);
                }
            }
            return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Unable to create environment");
        }

        public async Task<ActionResult> Restart()
        {
            StartCalculationRequest request = new StartCalculationRequest();
            request.Weight = new WeightInfo() { WeightUnit = EWeightUnit.Gram, WeightValue = 1537 };

            EnvironmentInfo environment = GetEnvironment();
            if (null != environment)
            {
                request.Environment = environment;
                PCalcResultInfo result = await m_Repository.Start(request);
                if (PcalcResultIsValid(result))
                {
                    AddOrUpdateTempData(result, request.Environment);
                    AddViewData(result, environment);
                    return View("Index", ProductCalculationViewModel.Create(result.QueryType));
                }
                return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), result);
            }
            return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Unable to create environment");
        }

        public async Task<ActionResult> StepBack()
        {
            PCalcResultInfo lastResult = GetLastPcalcResult();
            if (null != lastResult)
            {
                UpdateRequest request = new UpdateRequest();
                EnvironmentInfo environment = GetEnvironment();
                if (null != environment)
                {
                    request.Environment = environment;
                    request.ProductDescription = lastResult.ProductDescription;
                    PCalcResultInfo result = await m_Repository.StepBack(request);
                    if (PcalcResultIsValid(result))
                    {
                        AddOrUpdateTempData(result, request.Environment);
                        AddViewData(result, environment);
                        return View("Index", ProductCalculationViewModel.Create(result.QueryType));
                    }
                    return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), result);
                }
                return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Unable to create environment");
            }
            return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Unable to retrieve last result");
        }

        public async Task<ActionResult> Finish()
        {
            return null;
        }

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateWeight([Bind(Include = "WeightValueInGram,WeightValueInOunces,CultureIsMetric")]UpdateWeightViewModel model)
        {
            if(ModelState.IsValid)
            {
                PCalcResultInfo lastResult = GetLastPcalcResult();
                if (null != lastResult)
                {
                    UpdateRequest request = new UpdateRequest();
                    EnvironmentInfo environment = GetEnvironment();
                    if (null != environment)
                    {
                        request.Environment = environment;
                        request.ProductDescription = lastResult.ProductDescription;
                        int weightValue = (int)model.WeightValue * 10;
                        request.ProductDescription.Weight = new WeightInfo()
                        {
                            WeightValue = weightValue,
                            WeightUnit = model.CultureIsMetric ? EWeightUnit.TenthGram : EWeightUnit.TenthOunce
                        };

                        //update weight will only update the product description
                        PCalcResultInfo result = await m_Repository.UpdateWeight(request);
                        result.QueryDescription = lastResult.QueryDescription;
                        result.QueryType = lastResult.QueryType;
                        if (PcalcResultIsValid(result))
                        {
                            AddOrUpdateTempData(result, request.Environment);
                            AddViewData(result, environment);
                            return View("Index", ProductCalculationViewModel.Create(result.QueryType));
                        }
                        return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), result);
                    }
                    return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Unable to create environment");
                }
                return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Unable to retrieve last result");
            }
            return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Unspecified error");
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
            if (ModelState.IsValid)
            {
                EnvironmentInfo environment = GetEnvironment();
                if (null != environment)
                {
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
                return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Unable to retrieve environment information");
            }
            return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Product Calculation Error");
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
            if (null != lastResult)
            {
                if (null != environment)
                {
                    CalculateRequest calc = new CalculateRequest();
                    calc.Environment = environment;
                    calc.ProductDescription = lastResult.ProductDescription;
                    calc.ActionResult = actionResult;
                    PCalcResultInfo result = await m_Repository.Calculate(calc);
                    //TODO: Error handling
                    if (PcalcResultIsValid(result))
                    {
                        AddOrUpdateTempData(result, calc.Environment);
                        AddViewData(result, environment);
                        return View("Index", ProductCalculationViewModel.Create(result.QueryType));
                    }
                    return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), result);
                }
                return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Unable to retrieve environment information");
            }
            return HandleGeneralError("Index", ProductCalculationViewModel.Create(EQueryType.None), "Unable to retrieve last product");
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
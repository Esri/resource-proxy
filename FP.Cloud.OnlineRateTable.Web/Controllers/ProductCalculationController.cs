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
        public static readonly string PCALC_RESULT = "PcalcResult";
        public static readonly string REQUEST_DESCRIPTION = "RequestDescription";
        public static readonly string POSTAGE = "Postage";
        public static readonly string WEIGHT = "Weight";
        public static readonly string ENVIRONMENT = "Environment";
        public static readonly string GENERAL_ERROR = "GeneralError";
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
                ViewData.Add(ENVIRONMENT, allActive);
                return View(ProductCalculationViewModel.Create(EQueryType.None));
            }
            ModelState.AddModelError(GENERAL_ERROR, "Unable to retrieve active rate tables");
            return View("Index", ProductCalculationViewModel.Create(EQueryType.None));
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
                    if (null != result)
                    {
                        AddOrUpdateTempData(result, request.Environment);
                        AddViewData(result);
                        return View("Index", ProductCalculationViewModel.Create(result.QueryType));
                    }
                    ModelState.AddModelError(GENERAL_ERROR, "Unable to initialize product calculation");
                    return View("Index", ProductCalculationViewModel.Create(EQueryType.None));
                }
            }
            ModelState.AddModelError(GENERAL_ERROR, "Unable to create environment");
            return View("Index", ProductCalculationViewModel.Create(EQueryType.None));
        }

        public async Task<ActionResult> Restart()
        {
            StartCalculationRequest request = new StartCalculationRequest();
            request.Weight = new WeightInfo() { WeightUnit = EWeightUnit.Gram, WeightValue = 1537 };
            request.Environment = GetEnvironment();
            PCalcResultInfo result = await m_Repository.Start(request);
            AddOrUpdateTempData(result, request.Environment);
            AddViewData(result);
            return View("Index", ProductCalculationViewModel.Create(result.QueryType));
        }

        public async Task<ActionResult> StepBack()
        {
            PCalcResultInfo lastResult = GetLastPcalcResult();
            if (null != lastResult)
            {
                StepBackRequest request = new StepBackRequest();
                request.Environment = GetEnvironment();
                request.ProductDescription = lastResult.ProductDescription;
                PCalcResultInfo result = await m_Repository.StepBack(request);
                if (null != result)
                {
                    AddOrUpdateTempData(result, request.Environment);
                    AddViewData(result);
                    return View("Index", ProductCalculationViewModel.Create(result.QueryType));
                }
                ModelState.AddModelError(GENERAL_ERROR, "Unable to contact product calculation API");
                return View("Index", ProductCalculationViewModel.Create(EQueryType.None));
            }
            ModelState.AddModelError(GENERAL_ERROR, "Unable to retrieve last result");
            return View("Index", ProductCalculationViewModel.Create(EQueryType.None));
        }

        public async Task<ActionResult> Finish()
        {
            return null;
        }

        public async Task<ActionResult> UpdateWeight()
        {
            return null;
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
                ModelState.AddModelError(GENERAL_ERROR, "Unable to retrieve environment informationr");
                return View("Index", ProductCalculationViewModel.Create(EQueryType.None));
            }
            ModelState.AddModelError(GENERAL_ERROR, "Product Calculation Error");
            return View("Index", ProductCalculationViewModel.Create(EQueryType.None));
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
        public ActionResult FormatInput(string formatString, string inputString)
        {
            CultureInfo culture = new CultureInfo(GetEnvironment().Culture);
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

                    AddOrUpdateTempData(result, calc.Environment);
                    AddViewData(result);
                    return View("Index", ProductCalculationViewModel.Create(result.QueryType));
                }
                ModelState.AddModelError(GENERAL_ERROR, "Unable to retrieve environment informationr");
                return View("Index", ProductCalculationViewModel.Create(EQueryType.None));
            }
            ModelState.AddModelError(GENERAL_ERROR, "Unable to retrieve last product");
            return View("Index", ProductCalculationViewModel.Create(EQueryType.None));
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

        private void AddViewData(PCalcResultInfo result)
        {
            ViewData.Add(PCALC_RESULT, result);
            ViewData.Add(WEIGHT, result?.ProductDescription?.Weight);
            ViewData.Add(POSTAGE, result?.ProductDescription?.Postage);
            ViewData.Add(REQUEST_DESCRIPTION, result?.DedicatedDescription);
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
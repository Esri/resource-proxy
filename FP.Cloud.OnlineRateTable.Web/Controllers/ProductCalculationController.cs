using FP.Cloud.OnlineRateTable.Common.ProductCalculation;
using FP.Cloud.OnlineRateTable.Common.ProductCalculation.ApiRequests;
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
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Start()
        {
            StartCalculationRequest request = new StartCalculationRequest();
            request.Weight = new WeightInfo() { WeightUnit = EWeightUnit.Gram, WeightValue = 1537 };
            request.Environment = GetEnvironment();
            PCalcResultInfo result = await m_Repository.Start(request);
            AddOrUpdateTempData(result);
            ViewData.Add(PCALC_RESULT, result);
            return View("Index", new ProductCalculationViewModel(result.QueryType));
        }

        public async Task<ActionResult> StepBack()
        {
            PCalcResultInfo lastResult = GetLastPcalcResult();
            ProductCalculationViewModel viewModel = new ProductCalculationViewModel();
            if (null != lastResult)
            {
                StepBackRequest request = new StepBackRequest();
                request.Environment = GetEnvironment();
                request.ProductDescription = lastResult.ProductDescription;
                PCalcResultInfo result = await m_Repository.StepBack(request);

                viewModel.QueryType = result.QueryType;
                AddOrUpdateTempData(result);
                ViewData.Add(PCALC_RESULT, result);
                return View("Index", viewModel);
            }
            return View("Index", viewModel);
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
            return await HandleCalculation(actionResult);
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
            return await HandleCalculation(actionResult);
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
            return await HandleCalculation(actionResult);
        }

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RequestValue(ProductCalculationViewModel model)
        {
            CultureInfo culture = new CultureInfo(GetEnvironment().Culture);
            char decimalSeperator = culture.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0];

            FormatStringAdapter adapter = new FormatStringAdapter(decimalSeperator);
            adapter.SetFormatString(model.RequestValueModel.FormatString);

            string formattedString;
            adapter.Format(out formattedString, model.RequestValueModel.EnteredRawValue);

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
            return await HandleCalculation(actionResult);
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
            return await HandleCalculation(actionResult);
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
        private async Task<ActionResult> HandleCalculation(ActionResultInfo actionResult)
        {
            ProductCalculationViewModel viewModel = new ProductCalculationViewModel();
            PCalcResultInfo lastResult = GetLastPcalcResult();
            if (null != lastResult)
            {
                CalculateRequest calc = new CalculateRequest();
                calc.Environment = GetEnvironment();
                calc.ProductDescription = lastResult.ProductDescription;
                calc.ActionResult = actionResult;
                PCalcResultInfo result = await m_Repository.Calculate(calc);
                //TODO: Error handling

                viewModel.QueryType = result.QueryType;
                AddOrUpdateTempData(result);
                ViewData.Add(PCALC_RESULT, result);
                return View("Index", viewModel);
            }
            ModelState.AddModelError("GeneralError", "Unspecified error during product calculation");
            return View("Index", viewModel);
        }

        private void AddOrUpdateTempData(PCalcResultInfo result)
        {
            if (TempData.ContainsKey(PCALC_RESULT))
            {
                TempData[PCALC_RESULT] = result;
            }
            else
            {
                TempData.Add(PCALC_RESULT, result);
            }
        }

        private PCalcResultInfo GetLastPcalcResult()
        {
            if(TempData.Keys.Contains(PCALC_RESULT))
            {
                var result = (PCalcResultInfo)TempData[PCALC_RESULT];
                TempData.Keys.Remove(PCALC_RESULT);
                return result;
            }
            return null;
        }

        private EnvironmentInfo GetEnvironment()
        {
            return new EnvironmentInfo()
            {
                CarrierId = 1,
                Culture = "en-US",
                Iso3CountryCode = "USA",
                SenderZipCode = "12345",
                UtcDate = DateTime.Now.ToUniversalTime()
            };
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
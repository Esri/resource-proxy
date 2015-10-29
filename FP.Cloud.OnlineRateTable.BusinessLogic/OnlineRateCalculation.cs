using FP.Cloud.OnlineRateTable.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FP.Cloud.OnlineRateTable.Common;
using FP.Cloud.OnlineRateTable.Common.ProductCalculation;
using FP.Cloud.OnlineRateTable.PCalcLib;
using FP.Cloud.OnlineRateTable.Common.RateTable;
using System.IO;
using System.Globalization;
using FP.Cloud.OnlineRateTable.Common.ScenarioRunner;

namespace FP.Cloud.OnlineRateTable.BusinessLogic
{
    public class OnlineRateCalculation : IRateCalculation
    {
        #region members
        private RateCalculationFileHandler m_Handler;
        private ScenarioRunner m_ScenarioRunner;
        #endregion

        #region constructor
        public OnlineRateCalculation(RateCalculationFileHandler handler, ScenarioRunner runner)
        {
            m_Handler = handler;
            m_ScenarioRunner = runner;
        }
        #endregion

        #region IRateCalculation
        public async Task<PCalcResultInfo> StartCalculation(EnvironmentInfo environment, WeightInfo weight)
        {
            //lookup correct entry
            ScenarioResult initResult = await m_ScenarioRunner.RunAsync(InitFileHandler(environment));
            if (initResult.Success == false || m_Handler.IsValid == false)
            {
                return ReturnErrorResult(initResult);
            }
            ScenarioResult<PCalcResultInfo> startResult = m_ScenarioRunner.Run(() => Start(environment, weight, m_Handler));
            if(startResult.Success == false)
            {
                return ReturnErrorResult(startResult);
            }
            return startResult.Value;
        }

        public async Task<PCalcResultInfo> Calculate(EnvironmentInfo environment, ProductDescriptionInfo productDescription, ActionResultInfo actionResult)
        {
            //lookup correct entry            
            ScenarioResult initResult = await m_ScenarioRunner.RunAsync(InitFileHandler(environment));
            if (initResult.Success == false || m_Handler.IsValid == false)
            {
                return ReturnErrorResult(initResult);
            }
            ScenarioResult<PCalcResultInfo> calcResult = m_ScenarioRunner.Run(() => Calculate(environment, productDescription, actionResult, m_Handler));
            if (calcResult.Success == false)
            {
                return ReturnErrorResult(calcResult);
            }
            return calcResult.Value;
        }

        public async Task<PCalcResultInfo>StepBack(EnvironmentInfo environment, ProductDescriptionInfo productDescription)
        {
            //lookup correct entry
            ScenarioResult initResult = await m_ScenarioRunner.RunAsync(InitFileHandler(environment));
            if (initResult.Success == false || m_Handler.IsValid == false)
            {
                return ReturnErrorResult(initResult);
            }
            ScenarioResult<PCalcResultInfo> backResult = m_ScenarioRunner.Run(() => Back(environment, productDescription, m_Handler));
            if (backResult.Success == false)
            {
                return ReturnErrorResult(backResult);
            }
            return backResult.Value;
        }

        public async Task<PCalcResultInfo> UpdateWeight(EnvironmentInfo environment, ProductDescriptionInfo productDescription)
        {
            //lookup correct entry
            ScenarioResult initResult = await m_ScenarioRunner.RunAsync(InitFileHandler(environment));
            if (initResult.Success == false || m_Handler.IsValid == false)
            {
                return ReturnErrorResult(initResult);
            }
            ScenarioResult<PCalcResultInfo> updateWeightResult = m_ScenarioRunner.Run(() => UpdateWeight(environment, productDescription, m_Handler));
            if (updateWeightResult.Success == false)
            {
                return ReturnErrorResult(updateWeightResult);
            }
            return updateWeightResult.Value;
        }
        #endregion

        #region private
        private void HandleCurrencySymbol(PCalcResultInfo result, EnvironmentInfo environment)
        {
            if(null != result && null != result.ProductDescription && 
                null != result.ProductDescription.Postage && null != environment )
            {
                try
                {
                    CultureInfo info = new CultureInfo(environment.Culture);
                    result.ProductDescription.Postage.CurrencySymbol = info.NumberFormat.CurrencySymbol;
                    result.ProductDescription.Postage.CurrencyDecimalSeparator = info.NumberFormat.CurrencyDecimalSeparator;
                }
                catch(Exception)
                { }
            }
        }

        private PCalcResultInfo ReturnErrorResult(ScenarioResult scenarioResult)
        {
            return new PCalcResultInfo()
            {
                ApiRequestSucceeded = false,
                ErrorMessage = scenarioResult.Error != null ? scenarioResult.Error.Message : "Unknown Error"
            };
        }

        private async Task<ScenarioResult<RateCalculationFileHandler>> InitFileHandler(EnvironmentInfo info)
        {
            await m_Handler.Initialize(info);
            return new ScenarioResult<RateCalculationFileHandler>() { Success = true, Value = m_Handler };
        }

        private ScenarioResult<PCalcResultInfo> Start(EnvironmentInfo environment, WeightInfo weight, RateCalculationFileHandler handler)
        {
            using (var context = new PCalcProxyContext(handler.PawnFile, handler.RateTableFile, handler.AdditionalFiles))
            {
                IPCalcProxy proxy = context.Proxy;
                PCalcResultInfo result = proxy.Start(environment, weight);
                HandleCurrencySymbol(result, environment);
                return new ScenarioResult<PCalcResultInfo>() { Value = result };
            }
        }

        private ScenarioResult<PCalcResultInfo> Calculate(EnvironmentInfo environment, ProductDescriptionInfo description, ActionResultInfo actionResult, RateCalculationFileHandler handler)
        {
            using (var context = new PCalcProxyContext(handler.PawnFile, handler.RateTableFile, handler.AdditionalFiles))
            {
                IPCalcProxy proxy = context.Proxy;
                PCalcResultInfo result = proxy.Calculate(environment, description, actionResult);
                HandleCurrencySymbol(result, environment);
                return new ScenarioResult<PCalcResultInfo>() { Value = result };
            }
        }

        private ScenarioResult<PCalcResultInfo> Back(EnvironmentInfo environment, ProductDescriptionInfo description, RateCalculationFileHandler handler)
        {
            using (var context = new PCalcProxyContext(handler.PawnFile, handler.RateTableFile, handler.AdditionalFiles))
            {
                IPCalcProxy proxy = context.Proxy;
                PCalcResultInfo result = proxy.Back(environment, description);
                HandleCurrencySymbol(result, environment);
                return new ScenarioResult<PCalcResultInfo>() { Value = result };
            }
        }

        private ScenarioResult<PCalcResultInfo> UpdateWeight(EnvironmentInfo environment, ProductDescriptionInfo description, RateCalculationFileHandler handler)
        {
            using (var context = new PCalcProxyContext(handler.PawnFile, handler.RateTableFile, handler.AdditionalFiles))
            {
                IPCalcProxy proxy = context.Proxy;
                PCalcResultInfo result = proxy.Calculate(environment, description);
                HandleCurrencySymbol(result, environment);
                return new ScenarioResult<PCalcResultInfo>() { Value = result };
            }
        }
        #endregion
    }
}

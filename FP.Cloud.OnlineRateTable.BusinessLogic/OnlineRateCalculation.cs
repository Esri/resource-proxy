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

namespace FP.Cloud.OnlineRateTable.BusinessLogic
{
    public class OnlineRateCalculation : IRateCalculation
    {
        #region members
        private RateCalculationFileHandler m_Handler;
        #endregion

        #region constructor
        public OnlineRateCalculation(RateCalculationFileHandler handler)
        {
            m_Handler = handler;
        }
        #endregion

        #region IRateCalculation
        public async Task<PCalcResultInfo> StartCalculation(EnvironmentInfo environment, WeightInfo weight)
        {
            //lookup correct entry
            await m_Handler.Initialize(environment);
            if(m_Handler.IsValid)
            { 
                using (var context = new PCalcProxyContext(m_Handler.PawnFile, m_Handler.RateTableFile, m_Handler.AdditionalFiles))
                {
                    IPCalcProxy proxy = context.Proxy;
                    PCalcResultInfo result = proxy.Start(environment, weight);
                    HandleCurrencySymbol(result, environment);
                    return result;
                }
            }
            return null;
        }

        public async Task<PCalcResultInfo> Calculate(EnvironmentInfo environment, ProductDescriptionInfo productDescription, ActionResultInfo actionResult)
        {
            //lookup correct entry
            await m_Handler.Initialize(environment);
            if (m_Handler.IsValid)
            {
                using (var context = new PCalcProxyContext(m_Handler.PawnFile, m_Handler.RateTableFile, m_Handler.AdditionalFiles))
                {
                    IPCalcProxy proxy = context.Proxy;
                    PCalcResultInfo result = proxy.Calculate(environment, productDescription, actionResult);
                    HandleCurrencySymbol(result, environment);
                    return result;
                }
            }
            return null;
        }

        public async Task<PCalcResultInfo>StepBack(EnvironmentInfo environment, ProductDescriptionInfo productDescription)
        {
            //lookup correct entry
            await m_Handler.Initialize(environment);
            if (m_Handler.IsValid)
            {
                using (var context = new PCalcProxyContext(m_Handler.PawnFile, m_Handler.RateTableFile, m_Handler.AdditionalFiles))
                {
                    IPCalcProxy proxy = context.Proxy;
                    PCalcResultInfo result = proxy.Back(environment, productDescription);
                    HandleCurrencySymbol(result, environment);
                    return result;
                }
            }
            return null;
        }

        public async Task<PCalcResultInfo> UpdateWeight(EnvironmentInfo environment, ProductDescriptionInfo productDescription)
        {
            //lookup correct entry
            await m_Handler.Initialize(environment);
            if (m_Handler.IsValid)
            {
                using (var context = new PCalcProxyContext(m_Handler.PawnFile, m_Handler.RateTableFile, m_Handler.AdditionalFiles))
                {
                    IPCalcProxy proxy = context.Proxy;
                    PCalcResultInfo result = proxy.Calculate(environment, productDescription);
                    HandleCurrencySymbol(result, environment);
                    return result;
                }
            }
            return null;
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
        #endregion
    }
}

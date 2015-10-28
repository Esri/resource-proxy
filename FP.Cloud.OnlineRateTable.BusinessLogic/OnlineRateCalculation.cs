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
                    return proxy.Start(environment, weight);
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
                    return proxy.Calculate(environment, productDescription, actionResult);
                }
            }
            return null;
        }

        public async Task<PCalcResultInfo>StepBack(EnvironmentInfo environment, ProductDescriptionInfo productDescription)
        {
            throw new NotImplementedException();
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
                    return proxy.Calculate(environment, productDescription);
                }
            }
            return null;
        }
        #endregion
    }
}

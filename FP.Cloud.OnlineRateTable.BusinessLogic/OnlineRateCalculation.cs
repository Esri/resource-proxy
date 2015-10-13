using FP.Cloud.OnlineRateTable.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FP.Cloud.OnlineRateTable.Common;
using FP.Cloud.OnlineRateTable.Common.ProductCalculation;

namespace FP.Cloud.OnlineRateTable.BusinessLogic
{
    public class OnlineRateCalculation : IRateCalculation
    {
        public async Task<PCalcResultInfo> StartCalculation(EnvironmentInfo environment, WeightInfo weight)
        {
            throw new NotImplementedException();
        }

        public async Task<PCalcResultInfo> Calculate(EnvironmentInfo environment, ProductDescriptionInfo productDescription, ActionResultInfo actionResult)
        {
            throw new NotImplementedException();
        }

        public async Task<PCalcResultInfo>StepBack(EnvironmentInfo environment, ProductDescriptionInfo productDescription)
        {
            throw new NotImplementedException();
        }

        public async Task<PCalcResultInfo> UpdateWeight(EnvironmentInfo environment, ProductDescriptionInfo productDescription, WeightInfo newWeight)
        {
            throw new NotImplementedException();
        }
    }
}

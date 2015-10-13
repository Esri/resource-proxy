using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FP.Cloud.OnlineRateTable.Common.ProductCalculation;

namespace FP.Cloud.OnlineRateTable.Common.Interfaces
{
    public interface IRateCalculation
    {
        Task<PCalcResultInfo> StartCalculation(EnvironmentInfo environment, WeightInfo weight);
        Task<PCalcResultInfo> Calculate(EnvironmentInfo environment, ProductDescriptionInfo productDescription, ActionResultInfo actionResult);
        Task<PCalcResultInfo> StepBack(EnvironmentInfo environment, ProductDescriptionInfo productDescription);
        Task<PCalcResultInfo> UpdateWeight(EnvironmentInfo environment, ProductDescriptionInfo productDescription, WeightInfo newWeight);
    }
}

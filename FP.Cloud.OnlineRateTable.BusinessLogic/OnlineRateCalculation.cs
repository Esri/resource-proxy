using FP.Cloud.OnlineRateTable.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FP.Cloud.OnlineRateTable.Common;
using FP.Cloud.OnlineRateTable.Common.ProductCalculation;
using  FP.Cloud.OnlineRateTable.PCalcLib;

namespace FP.Cloud.OnlineRateTable.BusinessLogic
{
    public class OnlineRateCalculation : IRateCalculation
    {
        public async Task<PCalcResultInfo> StartCalculation(EnvironmentInfo environment, WeightInfo weight)
        {
            using (var context = new PCalcProxyContext(@"C:\Temp\Pt2097152.amx", @"C:\Temp\Pt2097152.bin"))
            {
                IPCalcProxy proxy = context.Proxy;
                return proxy.Calculate(environment, weight);
            }
        }

        public async Task<PCalcResultInfo> Calculate(EnvironmentInfo environment, ProductDescriptionInfo productDescription, ActionResultInfo actionResult)
        {
            using (var context = new PCalcProxyContext(@"C:\Temp\Pt2097152.amx", @"C:\Temp\Pt2097152.bin"))
            {
                IPCalcProxy proxy = context.Proxy;
                return proxy.Calculate(environment, productDescription, actionResult);
            }
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

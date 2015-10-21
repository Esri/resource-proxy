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
            return new PCalcResultInfo()
            {
                QueryType = EQueryType.ShowMenu,
                ProductDescription = new ProductDescriptionInfo()
                {
                    Postage = new PostageInfo() { CurrencySymbol = "$", PostageDecimals = 2, PostageValue = 200 },
                    ProductCode = 1,
                    ProductId = 34,
                    RateVersion = 2007654,
                    ScaleMode = EScaleMode.NO_SCALE,
                    State = EProductDescriptionState.Incomplete,
                    Weight = weight,
                    WeightClass = 1
                },
                QueryDescription = new ShowMenuDescriptionInfo()
                {
                    AdditionalInfo = "Additional Details",
                    DescriptionTitle = "Choose from the following entries",
                    MenuEntries = new List<string>() { "Entry0", "Entry1", "Entry2", "Entry3", "Entry4" }
                }
            };
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

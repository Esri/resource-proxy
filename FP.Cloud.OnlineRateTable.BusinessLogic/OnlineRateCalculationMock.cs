using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FP.Cloud.OnlineRateTable.Common.Interfaces;
using FP.Cloud.OnlineRateTable.Common.ProductCalculation;

namespace FP.Cloud.OnlineRateTable.BusinessLogic
{
    public class OnlineRateCalculationMock : IRateCalculation
    {
        #region Public Methods and Operators

        public async Task<PCalcResultInfo> Calculate(EnvironmentInfo environment, ProductDescriptionInfo productDescription, ActionResultInfo actionResult)
        {
            var result = new PCalcResultInfo();
            result.ProductDescription = productDescription;
            DescriptionInfo desc = GetRandomDescription();
            result.QueryType = GetQueryType(desc);
            result.QueryDescription = desc.ToTransferDescription();
            return result;
        }

        public async Task<PCalcResultInfo> StartCalculation(EnvironmentInfo environment, WeightInfo weight)
        {
            var result = new PCalcResultInfo();
            result.ProductDescription = new ProductDescriptionInfo()
                                            {
                                                Postage = new PostageInfo() { CurrencySymbol = "$", PostageDecimals = 2, PostageValue = 200 },
                                                ProductCode = 1,
                                                ProductId = 34,
                                                RateVersion = 2007654,
                                                ScaleMode = EScaleMode.NO_SCALE,
                                                State = EProductDescriptionState.Incomplete,
                                                Weight = weight,
                                                WeightClass = 1
                                            };
            DescriptionInfo desc = GetRandomDescription();
            result.QueryType = GetQueryType(desc);
            result.QueryDescription = desc.ToTransferDescription();
            return result;
        }

        public async Task<PCalcResultInfo> StepBack(EnvironmentInfo environment, ProductDescriptionInfo productDescription)
        {
            throw new NotImplementedException();
        }

        public async Task<PCalcResultInfo> UpdateWeight(EnvironmentInfo environment, ProductDescriptionInfo productDescription, WeightInfo newWeight)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Methods

        private EQueryType GetQueryType(DescriptionInfo info)
        {
            if (info is ShowMenuDescriptionInfo)
            {
                return EQueryType.ShowMenu;
            }
            if (info is SelectValueDescriptionInfo)
            {
                return EQueryType.SelectValue;
            }
            if (info is SelectIndexDescriptionInfo)
            {
                return EQueryType.SelectIndex;
            }
            if (info is RequestDescriptionInfo)
            {
                return EQueryType.RequestPostage;
            }
            else
            {
                return EQueryType.ShowDisplay;
            }
        }

        private DescriptionInfo GetRandomDescription()
        {
            DescriptionInfo[] descriptions = new DescriptionInfo[]
                                                 {
                                                     new ShowMenuDescriptionInfo()
                                                         {
                                                             AdditionalInfo = "Additional Details",
                                                             DescriptionTitle = "Choose from the following entries",
                                                             MenuEntries = new List<string>() { "Entry0", "Entry1", "Entry2", "Entry3", "Entry4" }
                                                         },
                                                     new SelectValueDescriptionInfo()
                                                         {
                                                             DescriptionTitle = "Please select a value",
                                                             ValueEntries =
                                                                 new List<ValueEntryInfo>()
                                                                     {
                                                                         new ValueEntryInfo() { EntryMessage = "Item1", EntryValue = 10 },
                                                                         new ValueEntryInfo() { EntryMessage = "Item2", EntryValue = 20 },
                                                                         new ValueEntryInfo() { EntryMessage = "Item3", EntryValue = 30 }
                                                                     }
                                                         },
                                                     new RequestDescriptionInfo() { DescriptionTitle = "Please enter a value", DisplayFormat = @"$%2u.%2u", Label = 2, StatusMessage = "Request Value Status message" },
                                                     new SelectIndexDescriptionInfo() { DescriptionTitle = "Please select an index", IndexEntries = new List<string>() { "IndexItem1", "IndexItem2", "IndexItem3" } },
                                                     new DescriptionInfo() { DescriptionTitle = "Please read and acknowledge this text", },
                                                 };
            int index = new Random().Next(0, descriptions.Length);
            return descriptions[index];
        }

        #endregion
    }
}
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
        /// <summary>
        /// Starts the rate calculation process.
        /// </summary>
        /// <param name="environment">The product table environment.</param>
        /// <param name="weight">The initial weight.</param>
        /// <returns></returns>
        Task<PCalcResultInfo> StartCalculation(EnvironmentInfo environment, WeightInfo weight);

        /// <summary>
        /// Performs a calculation step in the product calculation process
        /// </summary>
        /// <param name="environment">The product table environment.</param>
        /// <param name="productDescription">The description of the product prior to the calculation step.</param>
        /// <param name="actionResult">The step to be performed with the next calculation.</param>
        /// <returns></returns>
        Task<PCalcResultInfo> Calculate(EnvironmentInfo environment, ProductDescriptionInfo productDescription, ActionResultInfo actionResult);

        /// <summary>
        /// Performs one step back in the product calculation wizard
        /// </summary>
        /// <param name="environment">The product table environment.</param>
        /// <param name="productDescription">The description of the product prior to the step back.</param>
        /// <returns></returns>
        Task<PCalcResultInfo> StepBack(EnvironmentInfo environment, ProductDescriptionInfo productDescription);

        /// <summary>
        /// Updates the weight for the current product.
        /// </summary>
        /// <param name="environment">The product table environment.</param>
        /// <param name="productDescription">The description of the product prior to the update weight.</param>
        /// <returns></returns>
        Task<PCalcResultInfo> UpdateWeight(EnvironmentInfo environment, ProductDescriptionInfo productDescription);
    }
}

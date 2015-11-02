using FP.Cloud.OnlineRateTable.Common;
using FP.Cloud.OnlineRateTable.Common.Interfaces;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using FP.Cloud.OnlineRateTable.Common.ProductCalculation;
using FP.Cloud.OnlineRateTable.Common.ProductCalculation.ApiRequests;
using FP.Cloud.OnlineRateTable.Common.RateTable;

namespace FP.Cloud.OnlineRateTable.Controllers
{
    /// <summary>
    /// This controller provides all necessary functions to implement an FP style
    /// product calculation
    /// </summary>
    public class RateCalculationController : ApiController
    {
        
        #region members
        private IRateCalculation m_Calculator;
        private IRateTableManager m_Manager;
        #endregion

        public RateCalculationController(IRateCalculation calculator, IRateTableManager manager)
        {
            m_Calculator = calculator;
            m_Manager = manager;
        }

        #region public web API
        /// <summary>
        /// returns a list of rate tables from the database with only the currently active table for each variant
        /// </summary>
        /// <param name="clientUtcDate">if a value is specified only rate tables with a valid from
        /// date lower than the specified one are returned</param>
        [ResponseType(typeof(IEnumerable<RateTableInfo>))]
        [HttpGet]
        [ActionName("GetActiveTables")]
        public async Task<IHttpActionResult> GetActiveTables(DateTime clientUtcDate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            IEnumerable<RateTableInfo> result = await m_Manager.GetLatestForVariant(false, clientUtcDate);
            return Ok(result);
        }

        /// <summary>
        /// Creates a basic product table environment.
        /// </summary>
        /// <param name="rateTableId">The rate table identifier.</param>
        /// <returns></returns>
        [ResponseType(typeof(EnvironmentInfo))]
        [HttpGet]
        [ActionName("CreateEnvironment")]
        public async Task<IHttpActionResult> CreateEnvironment(int rateTableId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            RateTableInfo info = await m_Manager.GetById(rateTableId);
            if(null == info)
            {
                return NotFound();
            }
            return Ok(new EnvironmentInfo()
            {
                CarrierId = info.CarrierId,
                Culture = info.Culture,
                Iso3CountryCode = info.Variant.Length >= 3 ? info.Variant.Substring(0,3) : string.Empty,
                UtcDate = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Starts the rate calculation process.
        /// </summary>
        /// <param name="startRequest">Parameter container storing initial weight and product table environment.</param>
        /// <returns></returns>
        [ResponseType(typeof(PCalcResultInfo))]
        [HttpPost]
        [ActionName("Start")]
        public async Task<IHttpActionResult> StartProductCalculation(StartCalculationRequest startRequest )
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            PCalcResultInfo result = await m_Calculator.StartCalculation(startRequest.Environment, startRequest.Weight);
            return CreateProductCalculationResponse(result);
        }

        /// <summary>
        /// Performs a calculation step in the product calculation process
        /// </summary>
        /// <param name="calculationRequest">The calculation request storing the product table environment, 
        /// the description of the product prior to the calculation step and the step to be performed with the next calculation .</param>
        /// <returns></returns>
        [ResponseType(typeof(PCalcResultInfo))]
        [HttpPost]
        [ActionName("Calculate")]
        public async Task<IHttpActionResult> CalculateProduct(CalculateRequest calculationRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            PCalcResultInfo result = await m_Calculator.Calculate(calculationRequest.Environment, calculationRequest.ProductDescription, calculationRequest.ActionResult);
            return CreateProductCalculationResponse(result);
        }

        /// <summary>
        /// Performs one step back in the product calculation wizard
        /// </summary>
        /// <param name="backRequest">The step back request storing the product table environment 
        /// and the description of the product prior to the step back.</param>
        /// <returns></returns>
        [ResponseType(typeof(PCalcResultInfo))]
        [HttpPost]
        [ActionName("Back")]
        public async Task<IHttpActionResult> StepBack(UpdateRequest backRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            PCalcResultInfo result = await m_Calculator.StepBack(backRequest.Environment, backRequest.ProductDescription);
            return CreateProductCalculationResponse(result);
        }

        /// <summary>
        /// Updates the weight for the current product.
        /// </summary>
        /// <param name="updateRequest">The update request storing the product table environment and
        /// the description of the product prior to the update weight.</param>
        /// <returns></returns>
        [ResponseType(typeof(PCalcResultInfo))]
        [HttpPost]
        [ActionName("UpdateWeight")]
        public async Task<IHttpActionResult> UpdateWeight(UpdateRequest updateRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            PCalcResultInfo result = await m_Calculator.UpdateWeight(updateRequest.Environment, updateRequest.ProductDescription);
            return CreateProductCalculationResponse(result);
        }
        #endregion

        #region private
        private IHttpActionResult CreateProductCalculationResponse(PCalcResultInfo result)
        {
            if (null == result)
            {
                return BadRequest("Error in product calculation module");
            }
            return Ok(result);
        }
        #endregion
    }
}

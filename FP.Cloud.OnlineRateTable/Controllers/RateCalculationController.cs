using FP.Cloud.OnlineRateTable.Common;
using FP.Cloud.OnlineRateTable.Common.Interfaces;
using FP.Cloud.OnlineRateTable.Models;
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

namespace FP.Cloud.OnlineRateTable.Controllers
{
    public class RateCalculationController : ApiController
    {
        
        #region members
        private IRateCalculation m_Calculator;
        #endregion

        public RateCalculationController(IRateCalculation calculator)
        {
            m_Calculator = calculator;
        }

        #region public web API
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
            return Ok(result);
        }

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
            return Ok(result);
        }

        [ResponseType(typeof(PCalcResultInfo))]
        [HttpPost]
        [ActionName("Back")]
        public async Task<IHttpActionResult> StepBack(StepBackRequest backRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            PCalcResultInfo result = await m_Calculator.StepBack(backRequest.Environment, backRequest.ProductDescription);
            return Ok(result);
        }

        [ResponseType(typeof(PCalcResultInfo))]
        [HttpPost]
        [ActionName("UpdateWeight")]
        public async Task<IHttpActionResult> UpdateWeight(UpdateWeightRequest updateRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            PCalcResultInfo result = await m_Calculator.UpdateWeight(updateRequest.Environment, updateRequest.ProductDescription, updateRequest.NewWeight);
            return Ok(result);
        }
        #endregion
    }
}

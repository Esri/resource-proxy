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
        [ResponseType(typeof(IEnumerable<RateTableInfo>))]
        [HttpGet]
        [ActionName("GetActiveTables")]
        public async Task<IHttpActionResult> StartProductCalculation(DateTime clientUtcDate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            IEnumerable<RateTableInfo> result = await m_Manager.GetLatestForVariant(false, clientUtcDate);
            return Ok(result);
        }

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
        public async Task<IHttpActionResult> StepBack(UpdateRequest backRequest)
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
        public async Task<IHttpActionResult> UpdateWeight(UpdateRequest updateRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            PCalcResultInfo result = await m_Calculator.UpdateWeight(updateRequest.Environment, updateRequest.ProductDescription);
            return Ok(result);
        }
        #endregion
    }
}

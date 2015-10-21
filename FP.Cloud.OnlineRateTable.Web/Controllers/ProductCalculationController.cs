using FP.Cloud.OnlineRateTable.Common.ProductCalculation;
using FP.Cloud.OnlineRateTable.Common.ProductCalculation.ApiRequests;
using FP.Cloud.OnlineRateTable.Web.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FP.Cloud.OnlineRateTable.Web.Controllers
{
    public class ProductCalculationController : BaseController
    {
        #region members
        private ProductCalculationRepository m_Repository;
        #endregion

        #region constructor
        public ProductCalculationController(ProductCalculationRepository repository)
        {
            m_Repository = repository;
        }
        #endregion

        // GET: ProductCalculation
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Start()
        {
            StartCalculationRequest request = new StartCalculationRequest();
            request.Weight = new WeightInfo() { WeightUnit = EWeightUnit.Gram, WeightValue = 1537 };
            request.Environment = new EnvironmentInfo()
            {
                CarrierId = 1,
                Culture = "en-US",
                Iso3CountryCode = "USA",
                SenderZipCode = "12345",
                UtcDate = DateTime.Now.ToUniversalTime()
            };
            PCalcResultInfo result = await m_Repository.Start(request);
            return View("Index", result);

        }

        #region protected
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_Repository.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
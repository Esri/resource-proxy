using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using FP.Cloud.OnlineRateTable.Common.Interfaces;
using FP.Cloud.OnlineRateTable.Common.ProductCalculation;
using FP.Cloud.OnlineRateTable.Common.RateTable;
using FP.Cloud.OnlineRateTable.Models;

namespace FP.Cloud.OnlineRateTable.Controllers
{
    public class RateTableAdministrationController : ApiController
    {
        #region members
        private IRateTableManager m_Manager;
        #endregion

        #region constructor
        public RateTableAdministrationController(IRateTableManager manager)
        {
            m_Manager = manager;
        }
        #endregion

        #region public web API
        [ResponseType(typeof(IQueryable<RateTableInfo>))]
        [HttpGet]
        [ActionName("GetAll")]
        [Authorize(Roles = "RateTableAdministrators")]
        public IQueryable<RateTableInfo> GetAllRateTables()
        {
            return m_Manager.GetAll().AsQueryable();
        }

        [ResponseType(typeof(RateTableInfo))]
        [HttpGet]
        [ActionName("GetById")]
        [Authorize(Roles = "RateTableAdministrators")]
        public async Task<RateTableInfo> GetById(int id)
        {
            return await m_Manager.GetById(id);
        }

        [ResponseType(typeof(RateTableInfo))]
        [HttpPost]
        [ActionName("AddNew")]
        [Authorize(Roles = "RateTableAdministrators")]
        public async Task<IHttpActionResult> AddNewRateTable(RateTableInfo info)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            RateTableInfo resultEntry = await m_Manager.AddNew(info);
            return Ok(resultEntry);
        }
        #endregion
    }
}

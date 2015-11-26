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
using FP.Cloud.OnlineRateTable.Common.ProductCalculation.ApiRequests;

namespace FP.Cloud.OnlineRateTable.Controllers
{
    /// <summary>
    /// This controller provides all necessary functions to administer FP style
    /// product calculation rate tables. These methods are protected. For you to be able to 
   ///  administer rate tables you need to login using the FP authentication API 
   /// and your user account needs to be assigned to the correct role
    /// </summary>
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
        /// <summary>
        /// Returns all RateTables from the database
        /// </summary>
        /// <param name="includeFileData">if set to true the Package file data is appended (long transmission time)</param>
        /// <returns></returns>
        [ResponseType(typeof(IEnumerable<RateTableInfo>))]
        [HttpGet]
        [ActionName("GetAll")]
        [Authorize(Roles = "RateTableAdministrators")]
        public async Task<IEnumerable<RateTableInfo>> GetAllRateTables(bool includeFileData)
        {
            IEnumerable<RateTableInfo> t = await m_Manager.GetLatestForVariant(true, DateTime.UtcNow);
            return await m_Manager.GetAll(includeFileData);
        }

        /// <summary>
        /// returns a rate table specified by database id
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [ResponseType(typeof(RateTableInfo))]
        [HttpGet]
        [ActionName("GetById")]
        [Authorize(Roles = "RateTableAdministrators")]
        public async Task<RateTableInfo> GetById(int id)
        {
            return await m_Manager.GetById(id);
        }

        /// <summary>
        /// returns rate tables from the database using the filters provided. this method also includes paging mechanisms
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [ResponseType(typeof(IEnumerable<RateTableInfo>))]
        [HttpPost]
        [ActionName("GetFiltered")]
        [Authorize(Roles = "RateTableAdministrators")]
        public async Task<IEnumerable<RateTableInfo>> GetRateTables(GetRateTableRequest request)
        {
            return await m_Manager.GetFiltered(request.Variant, request.Version, request.Carrier, 
                request.MinValidFrom, request.MaxValidFrom, request.Culture, request.StartValue, request.ItemCount, request.IncludeFileData);
        }

        /// <summary>
        /// Adds a new RateTable to the database
        /// </summary>
        /// <param name="info">The new information.</param>
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

        /// <summary>
        /// updates an existing rate table info in the database
        /// </summary>
        /// <param name="info">The updated information.</param>
        /// <returns></returns>
        [ResponseType(typeof(RateTableInfo))]
        [HttpPost]
        [ActionName("Update")]
        [Authorize(Roles = "RateTableAdministrators")]
        public async Task<IHttpActionResult> UpdateTable(RateTableInfo info)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            RateTableInfo resultEntry = await m_Manager.Update(info);
            if(null == resultEntry)
            {
                return NotFound();
            }
            return Ok(resultEntry);
        }

        /// <summary>
        /// deletes an rate table database entry
        /// </summary>
        /// <param name="id">id of the rate table to be deleted</param>
        /// <returns></returns>
        [ResponseType(typeof(bool))]
        [HttpDelete]
        [ActionName("Delete")]
        [Authorize(Roles = "RateTableAdministrators")]
        public async Task<IHttpActionResult> DeleteById(int id)
        {
            if (await m_Manager.DeleteRateTable(id) == false)
            {
                return NotFound();
            }
            return Ok(true);
        }
        #endregion
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FP.Cloud.OnlineRateTable.Common.RateTable;
using FP.Cloud.OnlineRateTable.Web.Repositories;
using FP.Cloud.OnlineRateTable.Web.Models.ViewModels;
using FP.Cloud.OnlineRateTable.Web.Scenarios;
using FP.Cloud.OnlineRateTable.Common.ScenarioRunner;
using Ninject;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Linq;

namespace FP.Cloud.OnlineRateTable.Web.Controllers
{
    [Authorize(Roles = "RateTableAdministrators")]
    public class RateTableController : BaseController
    {
        #region members
        private RateTableRepository m_Repository;
        #endregion

        #region properties
        [Inject]
        public ExtractArchiveScenario ExtractScenario{get; set;}
        [Inject]
        public ReadMetaDataScenario ReadMetaScenario { get; set; }
        #endregion

        #region constructor
        public RateTableController(RateTableRepository repository)
        {
            m_Repository = repository;
        }
        #endregion
        // GET: RateTable
        public async Task<ActionResult> Index()
        {   
            return View(await m_Repository.GetAllRateTables(GetAuthToken()));
        }

        // GET: RateTable/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id.HasValue == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RateTableInfo rateTableInfo = await m_Repository.GetById(id.Value, GetAuthToken());
            if (rateTableInfo == null)
            {
                return HttpNotFound();
            }
            return View(rateTableInfo);
        }

        // GET: RateTable/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RateTable/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ValidFrom,Culture,ZipUpload")] RateTableViewModel rateTableViewModel, HttpPostedFileBase upload)
        {
            if (upload == null || upload.ContentLength == 0)
            {
                ModelState.AddModelError("ZipUpload", "This field is required");
            }
            else if (upload.ContentType != "application/x-zip-compressed")
            {
                ModelState.AddModelError("ZipUpload", "Please choose a zip archive.");
            }
            if (ModelState.IsValid)
            {
                //add basic stuff
                RateTableInfo rateTableInfo = new RateTableInfo();
                rateTableInfo.Culture = rateTableViewModel.Culture;
                rateTableInfo.ValidFrom = rateTableViewModel.ValidFrom;

                //add the files
                ScenarioResult<List<RateTableFileInfo>> extractResult = ExtractScenario.Execute(upload.InputStream);
                if(extractResult.Success)
                {
                    rateTableInfo.PackageFiles = extractResult.Value;
                    ScenarioResult metaResult = ReadMetaScenario.Execute(rateTableInfo);
                    if(metaResult.Success)
                    {
                        //parsing meta file succeeded - information is stored in rateTableInfo
                        //add new item to database
                        await m_Repository.AddNewRateTable(rateTableInfo, GetAuthToken());
                        return RedirectToAction("Index");
                    }
                }
                ModelState.AddModelError("ZipUpload", "Error processing RateTable file");
            }

            return View(rateTableViewModel);
        }

        // GET: RateTable/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id.HasValue == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RateTableInfo rateTableInfo = await m_Repository.GetById(id.Value, GetAuthToken());
            if (rateTableInfo == null)
            {
                return HttpNotFound();
            }
            return View(rateTableInfo);
        }

        // POST: RateTable/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Variant,VersionNumber,CarrierId,CarrierDetails,ValidFrom,Culture")] RateTableInfo rateTableInfo)
        {
            if (ModelState.IsValid)
            {
                await m_Repository.UpdateRateTable(rateTableInfo, GetAuthToken());
                return RedirectToAction("Index");
            }
            return View(rateTableInfo);
        }

        // GET: RateTable/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id.HasValue == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RateTableInfo rateTableInfo = await m_Repository.GetById(id.Value, GetAuthToken());
            if (rateTableInfo == null)
            {
                return HttpNotFound();
            }
            return View(rateTableInfo);
        }

        // POST: RateTable/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            RateTableInfo rateTableInfo = await m_Repository.GetById(id, GetAuthToken());
            if (null == rateTableInfo)
            {
                return HttpNotFound();
            }
            await m_Repository.DeleteRateTable(id, GetAuthToken());
            return RedirectToAction("Index");
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

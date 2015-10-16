using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FP.Cloud.OnlineRateTable.Common.RateTable;
using FP.Cloud.OnlineRateTable.Web.Repositories;

namespace FP.Cloud.OnlineRateTable.Web.Controllers
{
    public class RateTableController : Controller
    {
        private RateTableRepository m_Repository = new RateTableRepository();

        public async Task<ActionResult> Login()
        {
            UserRepository user = new UserRepository();
            await user.Login("k.nicolai@francotyp.com", "#Sicher01");
            return  RedirectToAction("Index");
        }
        // GET: RateTable
        public async Task<ActionResult> Index()
        {
            return View(await m_Repository.GetAllRateTables());
        }

        // GET: RateTable/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id.HasValue == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RateTableInfo rateTableInfo = await m_Repository.GetById(id.Value);
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
        public async Task<ActionResult> Create([Bind(Include = "Id,Variant,VersionNumber,CarrierId,CarrierDetails,ValidFrom,Culture")] RateTableInfo rateTableInfo)
        {
            if (ModelState.IsValid)
            {
                await m_Repository.AddNewRateTable(rateTableInfo);
                return RedirectToAction("Index");
            }

            return View(rateTableInfo);
        }

        // GET: RateTable/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id.HasValue == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RateTableInfo rateTableInfo = await m_Repository.GetById(id.Value);
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
                await m_Repository.UpdateRateTable(rateTableInfo);
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
            RateTableInfo rateTableInfo = await m_Repository.GetById(id.Value);
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
            RateTableInfo rateTableInfo = await m_Repository.GetById(id);
            if (null == rateTableInfo)
            {
                return HttpNotFound();
            }
            await m_Repository.DeleteRateTable(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_Repository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

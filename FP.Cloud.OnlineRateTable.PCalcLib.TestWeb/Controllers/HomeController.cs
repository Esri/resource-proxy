using System.Collections.Generic;
using System.Web.Mvc;
using FP.Cloud.OnlineRateTable.Common.ProductCalculation;

namespace FP.Cloud.OnlineRateTable.PCalcLib.TestWeb.Controllers
{
    public class HomeController : Controller
    {
        #region Public Methods and Operators

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Index()
        {
            string pawnFile = HttpContext.Server.MapPath("~/App_Data/Pt2097152.amx");
            string tableFile = HttpContext.Server.MapPath("~/App_Data/Pt2097152.bin");
            EnvironmentInfo env = new EnvironmentInfo() { Culture = "de", SenderZipCode = "123" };

            using (var context = new PCalcProxyContext(env, pawnFile, tableFile))
            {
                IPCalcProxy proxy = context.Proxy;

                PCalcResultInfo result = null;

                result = proxy.Start(env, new WeightInfo() { WeightUnit = EWeightUnit.Gram, WeightValue = 20 });
                result = proxy.Calculate(env, result.ProductDescription, new ActionResultInfo() { Action = EActionId.ShowMenu, Label = 0, Results = new List<AnyInfo> { new AnyInfo() { AnyType = EAnyType.INT32, AnyValue = "0" } } });
                result = proxy.Calculate(env, result.ProductDescription, new ActionResultInfo() { Action = EActionId.ShowMenu, Label = 0, Results = new List<AnyInfo> { new AnyInfo() { AnyType = EAnyType.INT32, AnyValue = "0" } } });
            }

            return View();
        }

        #endregion
    }
}
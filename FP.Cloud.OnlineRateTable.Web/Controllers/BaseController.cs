using FP.Cloud.OnlineRateTable.Common.ProductCalculation;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace FP.Cloud.OnlineRateTable.Web.Controllers
{
    public class BaseController : Controller
    {
        #region const
        protected const string TOKEN_CLAIM = "TokenClaim";
        public static readonly string GENERAL_ERROR = "GeneralError";
        #endregion

        #region properties
        protected IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }
        #endregion

        #region protected
        protected string GetAuthToken()
        {
            Claim claim = AuthenticationManager.User.Claims.FirstOrDefault(c => c.Type == TOKEN_CLAIM);
            return claim?.Value;
        }

        protected ViewResult HandleGeneralError(string viewName, object model, string errorMessage)
        {
            ModelState.AddModelError(GENERAL_ERROR, errorMessage);
            return View(viewName, model);
        }

        protected ViewResult HandleGeneralError(string viewName, object model, PCalcResultInfo result)
        {
            if(null == result || null == result.CalculationError || string.IsNullOrEmpty(result.CalculationError.ErrorMessage))
            {
                return HandleGeneralError(viewName, model, "Error contacting product calculation");
            }
            ModelState.AddModelError(GENERAL_ERROR, string.Format("The following error occured: {0}", result.CalculationError.ErrorMessage));
            return View(viewName, model);
        }
        #endregion
    }
}
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
        #endregion
    }
}
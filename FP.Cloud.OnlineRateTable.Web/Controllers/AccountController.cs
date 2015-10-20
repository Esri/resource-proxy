using FP.Cloud.OnlineRateTable.Common.Authorization;
using FP.Cloud.OnlineRateTable.Web.Repositories;
using FP.Cloud.OnlineRateTable.Web.Scenarios;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.DataHandler.Serializer;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FP.Cloud.OnlineRateTable.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        #region const
        private const string COOKIE_NAME = "FP.Cloud.OnlineRateTable.Authorization.Auth";
        #endregion

        #region members
        private UserRepository m_UserRepository;
        #endregion

        #region constructor
        public AccountController(UserRepository userRepository)
        {
            m_UserRepository = userRepository;
        }
        #endregion

        #region properties
        [Inject]
        public CreateAuthCookieScenario AuthCookieScenario { get; set; }
        #endregion

        // GET: Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await m_UserRepository.Register(model);
            return View("Registered");
        }

        // GET: Account/SignIn
        [AllowAnonymous]
        public ActionResult SignIn(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Account/SignIn
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignIn(SignInModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            AccessToken result = await m_UserRepository.Login(model.Email, model.Password);
            if ((null != result) && (string.IsNullOrEmpty(result.TokenValue) == false))
            {
                var cookieResult = AuthCookieScenario.Execute(result, model.RememberMe, returnUrl, COOKIE_NAME);
                if (cookieResult.Success)
                {
                    //Let's keep the user authenticated in the MVC webapp.
                    //By using the AccessToken, we can use User.Identity.Name in the MVC controllers to make API calls.
                    FormsAuthentication.SetAuthCookie(result.TokenValue, model.RememberMe);

                    //And now, we have the cookie.
                    Response.SetCookie(cookieResult.Value);
                }
            }

            return Redirect(returnUrl ?? "/");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();

            //Clear the auth cookie
            if (Response.Cookies[COOKIE_NAME] != null)
            {
                var c = new HttpCookie(COOKIE_NAME) { Expires = DateTime.Now.AddDays(-1) };
                Response.Cookies.Add(c);
            }

            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_UserRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
using FP.Cloud.OnlineRateTable.Common.Authorization;
using FP.Cloud.OnlineRateTable.Web.Repositories;
using FP.Cloud.OnlineRateTable.Web.Scenarios;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FP.Cloud.OnlineRateTable.Web.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        #region members
        private UserRepository m_UserRepository;
        #endregion

        #region constructor
        public AccountController(UserRepository userRepository)
        {
            m_UserRepository = userRepository;
        }
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

            ApiResponse<object> response = await m_UserRepository.Register(model);
            return HandleApiResponse(response, View("Registered"), new HttpStatusCodeResult(HttpStatusCode.BadRequest));
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
            ApiResponse<AccessToken> signInResponse = await m_UserRepository.Login(model.Email, model.Password);
            if(IsApiError(signInResponse))
            {
                return HandleApiError(signInResponse, new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
            if (string.IsNullOrEmpty(signInResponse.ApiResult.TokenValue) == false)
            {
                ApiResponse<List<UserClaim>> userClaimResponse = await m_UserRepository.GetUserClaims(signInResponse.ApiResult.TokenValue);
                if(IsApiError(userClaimResponse))
                {
                    return HandleApiError(userClaimResponse, new HttpStatusCodeResult(HttpStatusCode.BadRequest));
                }

                List<Claim> claimList = userClaimResponse.ApiResult.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
                claimList.Add(new Claim(TOKEN_CLAIM, signInResponse.ApiResult.TokenValue));
                var id = new ClaimsIdentity(claimList, DefaultAuthenticationTypes.ApplicationCookie); 
                AuthenticationManager.SignIn(id);
            }

            return Redirect(returnUrl ?? "/");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignOut()
        {
            await m_UserRepository.Logout(GetAuthToken());
            AuthenticationManager.SignOut();

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
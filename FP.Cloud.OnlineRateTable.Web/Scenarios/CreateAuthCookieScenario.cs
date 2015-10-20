using FP.Cloud.OnlineRateTable.Common.Authorization;
using FP.Cloud.OnlineRateTable.Common.ScenarioRunner;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.DataHandler.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Security;

namespace FP.Cloud.OnlineRateTable.Web.Scenarios
{
    public class CreateAuthCookieScenario
    {
        #region members
        private ScenarioRunner m_ScenarioRunner;
        #endregion

        #region constructor
        public CreateAuthCookieScenario(ScenarioRunner scenarioRunner)
        {
            m_ScenarioRunner = scenarioRunner;
        }
        #endregion

        #region public
        public ScenarioResult<HttpCookie> Execute(AccessToken token, bool rememberMe, string redirectUri, string cookieName)
        {
            return m_ScenarioRunner.Run(() => CreateCookie(token, rememberMe, redirectUri, cookieName));
        }
        #endregion

        #region private
        public ScenarioResult<HttpCookie> CreateCookie(AccessToken token, bool rememberMe, string redirectUri, string cookieName)
        {
            //Create an AuthenticationTicket to generate a cookie used to authenticate against Web API.
            //But before we can do that, we need a ClaimsIdentity that can be authenticated in Web API.
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, token.UserName), //Name is the default name claim type, and UserName is the one known also in Web API.
                new Claim(ClaimTypes.NameIdentifier, token.UserName) //If you want to use User.Identity.GetUserId in Web API, you need a NameIdentifier claim.
            };

            //Generate a new ClaimsIdentity, using the DefaultAuthenticationTypes.ApplicationCookie authenticationType.
            //This also matches what we've set up in Web API.
            var authTicket = new AuthenticationTicket(new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie), new AuthenticationProperties
            {
                ExpiresUtc = token.ExpireTimestamp,
                IsPersistent = rememberMe,
                IssuedUtc = token.IssueTimestamp,
                RedirectUri = redirectUri
            });

            //And now it's time to generate the cookie data. This is using the same code that is being used by the CookieAuthenticationMiddleware class in OWIN.
            byte[] userData = DataSerializers.Ticket.Serialize(authTicket);

            //Protect this user data and add the extra properties. These need to be the same as in Web API!
            byte[] protectedData = MachineKey.Protect(userData, new[] { "Microsoft.Owin.Security.Cookies.CookieAuthenticationMiddleware", DefaultAuthenticationTypes.ApplicationCookie, "v1" });

            //base64-encode this data.
            string protectedText = TextEncodings.Base64Url.Encode(protectedData);

            //And now, we have the cookie.
            HttpCookie cookie = new HttpCookie(cookieName)
            {
                HttpOnly = true,
                Expires = token.ExpireTimestamp.UtcDateTime,
                Value = protectedText
            };
            return new ScenarioResult<HttpCookie>() { Success = true, Value = cookie };
        }
        #endregion
    }
}
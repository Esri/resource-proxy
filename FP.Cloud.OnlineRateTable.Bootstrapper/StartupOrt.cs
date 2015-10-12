using FP.Cloud.OnlineRateTable.Data;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP.Cloud.OnlineRateTable.Bootstrapper
{
    public class StartupOrt
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }

        public static void ConfigureAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext(ApplicationDbContext.Create);

            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
            //Token Consumption
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);

        }
    }
}

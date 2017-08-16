using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup("FP.Cloud.OnlineRateTable.Authorization", typeof(FP.Cloud.OnlineRateTable.Authorization.Startup))]

namespace FP.Cloud.OnlineRateTable.Authorization
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

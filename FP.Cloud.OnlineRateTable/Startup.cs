using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FP.Cloud.OnlineRateTable.Bootstrapper;

[assembly: OwinStartup("FP.Cloud.OnlineRateTable", typeof(FP.Cloud.OnlineRateTable.Startup))]

namespace FP.Cloud.OnlineRateTable
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            StartupOrt.ConfigureAuth(app);
        }
    }
}
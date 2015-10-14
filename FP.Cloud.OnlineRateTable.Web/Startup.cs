using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FP.Cloud.OnlineRateTable.Web.Startup))]
namespace FP.Cloud.OnlineRateTable.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}

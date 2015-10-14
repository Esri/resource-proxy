using System.Web;
using System.Web.Mvc;

namespace FP.Cloud.OnlineRateTable.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}

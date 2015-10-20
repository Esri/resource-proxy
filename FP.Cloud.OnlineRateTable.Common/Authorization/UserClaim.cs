using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FP.Cloud.OnlineRateTable.Common.Authorization
{
    public class UserClaim
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}

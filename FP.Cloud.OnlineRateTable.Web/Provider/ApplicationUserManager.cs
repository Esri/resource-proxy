using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;

namespace FP.Cloud.OnlineRateTable.Web.Provider
{
    public class ApplicationUserManager
    {
        internal Task<string> GetPhoneNumberAsync(string userId)
        {
            throw new NotImplementedException();
        }

        internal Task<bool> GetTwoFactorEnabledAsync(string userId)
        {
            throw new NotImplementedException();
        }

        internal Task<IList<UserLoginInfo>> GetLoginsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        internal Task<string> GetPhoneNumberAsync()
        {
            throw new NotImplementedException();
        }

        internal Task<bool> GetTwoFactorEnabledAsync()
        {
            throw new NotImplementedException();
        }

        internal Task<IList<UserLoginInfo>> GetLoginsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
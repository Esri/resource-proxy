using FP.Cloud.OnlineRateTable.Common.Authorization;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace FP.Cloud.OnlineRateTable.Web.Repositories
{
    public class UserRepository : RateTableApi, IDisposable
    {
        #region members
        private string m_TokenApi = ConfigurationManager.AppSettings["AuthorizationUrl"];
        private string m_ManageApi = string.Format("{0}/api/Account", ConfigurationManager.AppSettings["AuthorizationUrl"]);
        #endregion

        #region public
        public async Task<ApiResponse<AccessToken>> Login(string user, string password)
        {
            RestRequest request = GetNewRequest();
            request.Resource = "Token";
            request.AddParameter("grant_type", "password", ParameterType.GetOrPost);
            request.AddParameter("username", user, ParameterType.GetOrPost);
            request.AddParameter("password", password, ParameterType.GetOrPost);
            request.Method = Method.POST;

            return await Execute<AccessToken>(request, m_TokenApi, string.Empty);
        }

        public async Task Logout(string authToken)
        {
            RestRequest request = GetNewRequest();
            request.Resource = "Logout";
            request.Method = Method.POST;

            await Execute<object>(request, m_ManageApi, authToken);
        }

        public async Task<ApiResponse<EmptyObject>> Register(RegisterBindingModel model)
        {
            RestRequest request = GetNewRequest();
            request.Resource = "Register";
            request.Method = Method.POST;
            request.AddObject(model);
            return await Execute<EmptyObject>(request, m_ManageApi, string.Empty);
        }

        public async Task<ApiResponse<List<UserClaim>>> GetUserClaims(string authToken)
        {
            RestRequest request = GetNewRequest();
            request.Resource = "UserClaims";
            request.Method = Method.GET;
            return await Execute<List<UserClaim>>(request, m_ManageApi, authToken);
        }

        public async Task<ApiResponse<ManageInfoViewModel>> GetManageInfo(string returnUrl, string authToken)
        {
            RestRequest request = GetNewRequest();
            request.Resource = "ManageInfo";
            request.Method = Method.GET;
            return await Execute<ManageInfoViewModel>(request, m_ManageApi, authToken);

        }

        public async Task<ApiResponse<EmptyObject>>ChangePassword(ChangePasswordBindingModel model, string authToken)
        {
            RestRequest request = GetNewRequest();
            request.Resource = "ChangePassword";
            request.Method = Method.POST;
            request.AddObject(model);
            return await Execute<EmptyObject>(request, m_ManageApi, authToken);
        }
        #endregion

        #region IDisposable Support
        private bool m_DisposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!m_DisposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                m_DisposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UserRepository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
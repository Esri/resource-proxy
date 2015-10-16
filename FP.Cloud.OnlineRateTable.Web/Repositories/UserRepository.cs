using FP.Cloud.OnlineRateTable.Web.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
        public async Task Login(string user, string password)
        {
            m_Request.Resource = "Token";
            m_Request.AddParameter("grant_type", "password", ParameterType.GetOrPost);
            m_Request.AddParameter("username", user, ParameterType.GetOrPost);
            m_Request.AddParameter("password", password, ParameterType.GetOrPost);
            m_Request.Method = Method.POST;

            AccessToken t = await Execute<AccessToken>(m_TokenApi, false);

            if(null != t && string.IsNullOrEmpty(t.TokenValue) == false)
            {
                HttpContext.Current.Session[FP_ACCESS_TOKEN] = t.TokenValue;
            }
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
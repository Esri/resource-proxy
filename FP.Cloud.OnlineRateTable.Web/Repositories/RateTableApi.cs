using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace FP.Cloud.OnlineRateTable.Web.Repositories
{
    public class RateTableApi
    {
        #region constants
        protected readonly string FP_ACCESS_TOKEN = "FpAccessToken";
        protected readonly RestRequest m_Request = new RestRequest();
        #endregion

        #region constructor
        protected RateTableApi()
        {
            m_Request.RequestFormat = DataFormat.Json;
        }
        #endregion

        public async Task<T> Execute<T>(string uri, bool authenticationRequired) where T : new()
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(uri);

            if(authenticationRequired)
            {
                //read token from storage
                string token = (string)HttpContext.Current.Session[FP_ACCESS_TOKEN];
                //add bearer token to header
                m_Request.AddHeader("Authorization", string.Format("Bearer {0}", token));
            }
            
            var response = await client.ExecuteTaskAsync<T>(m_Request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                throw new ApplicationException(message, response.ErrorException);
            }
            return response.Data;
        }
    }
}
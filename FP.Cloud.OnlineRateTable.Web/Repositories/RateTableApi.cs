﻿using RestSharp;
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
        #endregion

        #region protected
        protected RestRequest GetNewRequest()
        {
            RestRequest request = new RestRequest();
            request.RequestFormat = DataFormat.Json;
            return request;
        }

        protected async Task<T> Execute<T>(RestRequest request, string uri, string authToken) where T : new()
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(uri);

            if(string.IsNullOrEmpty(authToken) == false)
            {
                //add bearer token to header
                request.AddHeader("Authorization", string.Format("Bearer {0}", authToken));
            }
            
            var response = await client.ExecuteTaskAsync<T>(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                throw new ApplicationException(message, response.ErrorException);
            }
            return response.Data;
        }
        #endregion
    }
}
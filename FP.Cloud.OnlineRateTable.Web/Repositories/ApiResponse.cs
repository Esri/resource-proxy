using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace FP.Cloud.OnlineRateTable.Web.Repositories
{
    public class ApiResponse<T> where T : new()
    {
        #region properties
        public bool ApiCallSucceeded { get; private set; }

        public HttpStatusCode? StatusCode { get; private set; }

        public T ApiResult { get; private set; }

        public string ErrorMessage{get; set;}
        #endregion

        #region constructor
        public ApiResponse()
        {
                
        }

        public ApiResponse(T result)
        {
            ApiCallSucceeded = true;
            ApiResult = result;
        }

        public ApiResponse(HttpStatusCode? code, params string[] messages)
        {
            ApiCallSucceeded = false;
            StatusCode = code;
            ErrorMessage = CreateErrorMessage(null, messages);
        }

        public ApiResponse(params string[] messages)
        {
            ApiCallSucceeded = false;
            ErrorMessage = CreateErrorMessage(null, messages);
        }
        #endregion

        #region private
        private string CreateErrorMessage(HttpStatusCode? code, params string[] messages)
        {
            StringBuilder sb = new StringBuilder();
            foreach(string s in messages)
            {
                sb.AppendLine(s);
            }
            if(code.HasValue)
            {
                sb.AppendLine(string.Format("Status Code: {0}", code.Value));
            }
            return sb.ToString();
        }
        #endregion
    }
}
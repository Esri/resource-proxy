using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FP.Cloud.OnlineRateTable.Common
{
    [Serializable]
    [DataContract]
    public class ApiResponse
    {
        #region properties
        [DataMember]
        public bool ApiRequestSucceeded { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }

        [DataMember]
        public int ErrorCode { get; set; }

        [DataMember]
        public int ErrorSubCode1 { get; set; }

        [DataMember]
        public int ErrorSubCode2 { get; set; }
        #endregion

        #region constructor
        public ApiResponse()
        {
            ApiRequestSucceeded = true;
        }

        public ApiResponse(string errorMessage, int errorCode, int errorSubCode1, int errorSubcode2)
        {
            ApiRequestSucceeded = false;
            ErrorMessage = errorMessage;
            ErrorSubCode1 = errorSubCode1;
            ErrorSubCode2 = errorSubcode2;
        }

        public ApiResponse(Exception ex)
        {
            ApiRequestSucceeded = false;
            ErrorMessage = ex.Message;
        }
        #endregion
    }
}

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
    public class ApiError
    {
        #region properties

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
        public ApiError()
        {
        }

        public ApiError(string errorMessage, int errorCode, int errorSubCode1, int errorSubcode2)
        {
            ErrorMessage = errorMessage;
            ErrorSubCode1 = errorSubCode1;
            ErrorSubCode2 = errorSubcode2;
        }

        public ApiError(Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        #endregion
    }
}

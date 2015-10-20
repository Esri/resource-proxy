using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FP.Cloud.OnlineRateTable.Common.Authorization
{
    public class AccessToken
    {
        [DeserializeAs(Name = "access_token")]
        public string TokenValue { get; set; }

        [DeserializeAs(Name = "toke_type")]
        public string TokenType { get; set; }

        [DeserializeAs(Name = "expires_in")]
        public uint LifeSpan { get; set; }

        [DeserializeAs(Name = "userName")]
        public string UserName { get; set; }

        [DeserializeAs(Name = ".issued")]
        public DateTimeOffset IssueTimestamp { get; set; }

        [DeserializeAs(Name = ".expires")]
        public DateTimeOffset ExpireTimestamp { get; set; }
    }
}
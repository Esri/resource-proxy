using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FP.Cloud.OnlineRateTable.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        [Authorize]
        public IEnumerable<string> Get()
        {
            var identity = User.Identity;
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [Authorize(Roles ="Administrators")]
        public string Get(int id)
        {
            var identity = User.Identity;
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}

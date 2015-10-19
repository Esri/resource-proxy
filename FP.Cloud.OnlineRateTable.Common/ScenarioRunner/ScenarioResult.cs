using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP.Cloud.OnlineRateTable.Common.ScenarioRunner
{
    public class ScenarioResult
    {
        public bool Success { get; set; }
    }

    public class ScenarioResult<T> : ScenarioResult
    {
        public T Value { get; set; }
    }
}

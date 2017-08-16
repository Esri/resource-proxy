using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FP.Cloud.OnlineRateTable.Common.RateTable;

namespace FP.Cloud.OnlineRateTable.Common.ScenarioRunner
{
    public class ScenarioRunner
    {
        #region Public Methods and Operators
        public ScenarioResult Run(Action action)
        {
            try
            {
                action();
                return new ScenarioResult() { Success = true };
            }
            catch(Exception e)
            {
                //ToDo: error handling
                return new ScenarioResult() { Success = false, Error = e };
            }
        }

        public async Task<ScenarioResult> RunAsync(Task<Action> action)
        {
            try
            {
                await action;
                return new ScenarioResult() { Success = true };
            }
            catch (Exception e)
            {
                //ToDo: error handling
                return new ScenarioResult() { Success = false, Error = e };
            }
        }

        public ScenarioResult<T> Run<T>(Func<ScenarioResult<T>> action)
        {
            try
            {
                ScenarioResult<T> result = action();
                result.Success = true;
                return result;
            }
            catch (Exception e)
            {
                //ToDo: error handling
                return new ScenarioResult<T>() { Success = false, Error = e };
            }
        }

        public async Task<ScenarioResult<T>> RunAsync<T>(Task<ScenarioResult<T>> action)
        {
            try
            {
                ScenarioResult<T> result = await action;
                result.Success = true;
                return result;
            }
            catch (Exception e)
            {
                //ToDo: error handling
                return new ScenarioResult<T>() { Success = false, Error = e };
            }
        }
        #endregion
    }
}

using FP.Cloud.OnlineRateTable.Common.ProductCalculation;
using FP.Cloud.OnlineRateTable.Common.ProductCalculation.ApiRequests;
using FP.Cloud.OnlineRateTable.Common.RateTable;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace FP.Cloud.OnlineRateTable.Web.Repositories
{
    public class ProductCalculationRepository : RateTableApi, IDisposable
    {
        #region members
        private string m_Api = string.Format("{0}/api/RateCalculation", ConfigurationManager.AppSettings["RateTableUrl"]);
        #endregion

        #region public
        public async Task<ApiResponse<PCalcResultInfo>> Start(StartCalculationRequest startRequest)
        {
            RestRequest request = GetNewRequest();
            request.Resource = "Start";
            request.Method = Method.POST;
            request.AddBody(startRequest);

            return await Execute<PCalcResultInfo>(request, m_Api, string.Empty);
        }

        public async Task<ApiResponse<PCalcResultInfo>> StepBack(UpdateRequest stepBackRequest)
        {
            RestRequest request = GetNewRequest();
            request.Resource = "Back";
            request.Method = Method.POST;
            request.AddBody(stepBackRequest);

            return await Execute<PCalcResultInfo>(request, m_Api, string.Empty);
        }

        public async Task<ApiResponse<PCalcResultInfo>> UpdateWeight(UpdateRequest updateWeightRequest)
        {
            RestRequest request = GetNewRequest();
            request.Resource = "UpdateWeight";
            request.Method = Method.POST;
            request.AddBody(updateWeightRequest);

            return await Execute<PCalcResultInfo>(request, m_Api, string.Empty);
        }

        public async Task<ApiResponse<PCalcResultInfo>> Calculate(CalculateRequest calculateRequest)
        {
            RestRequest request = GetNewRequest();
            request.Resource = "Calculate";
            request.Method = Method.POST;
            request.AddBody(calculateRequest);

            return await Execute<PCalcResultInfo>(request, m_Api, string.Empty);
        }

        public async Task<ApiResponse<List<RateTableInfo>>> GetActiveRateTables(DateTime clientDate)
        {
            DateTime clientUtcDate = clientDate.ToUniversalTime();

            RestRequest request = GetNewRequest();
            request.Resource = "GetActiveTables";
            request.Method = Method.GET;
            request.AddParameter("clientUtcDate", clientUtcDate, ParameterType.GetOrPost);

            return await Execute<List<RateTableInfo>>(request, m_Api, string.Empty);
        }

        public async Task<ApiResponse<EnvironmentInfo>> CreateEnvironment(int rateTableId)
        {
            RestRequest request = GetNewRequest();
            request.Resource = "CreateEnvironment";
            request.Method = Method.GET;
            request.AddParameter("rateTableId", rateTableId, ParameterType.GetOrPost);

            return await Execute<EnvironmentInfo>(request, m_Api, string.Empty);
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
        // ~RateTableRepository() {
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
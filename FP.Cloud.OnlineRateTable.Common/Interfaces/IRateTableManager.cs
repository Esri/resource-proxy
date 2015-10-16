using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FP.Cloud.OnlineRateTable.Common.RateTable;

namespace FP.Cloud.OnlineRateTable.Common.Interfaces
{
    public interface IRateTableManager
    {
        Task<RateTableInfo> AddNew(RateTableInfo newInfo);
        Task<RateTableInfo> Update(RateTableInfo updatedInfo);
        Task<bool> DeleteRateTable(int id);
        Task<IEnumerable<RateTableInfo>> GetAll();
        Task<RateTableInfo> GetById(int id);
        Task<IEnumerable<RateTableInfo>> GetFiltered(string variant, string version, int? carrier, DateTime? validFrom, string culture, int start, int count);
    }
}
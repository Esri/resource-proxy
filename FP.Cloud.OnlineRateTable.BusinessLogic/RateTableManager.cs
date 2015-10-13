using FP.Cloud.OnlineRateTable.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FP.Cloud.OnlineRateTable.Common.Interfaces;
using FP.Cloud.OnlineRateTable.Common.RateTable;

namespace FP.Cloud.OnlineRateTable.BusinessLogic
{
    public class RateTableManager : IRateTableManager
    {
        #region members
        private ApplicationDbContext m_DbContext;
        #endregion

        #region constructor
        public RateTableManager(ApplicationDbContext context)
        {
            m_DbContext = context;
        }
        #endregion

        #region public
        public IEnumerable<RateTableInfo> GetAll()
        {
            return m_DbContext.RateTables.Select(r => r.ToRateTableInfo());
        }

        public async Task<RateTableInfo> GetById(int id)
        {
            RateTable table = await m_DbContext.RateTables.FirstOrDefaultAsync(r => r.Id == id);
            return table?.ToRateTableInfo();
        }

        public async Task<IEnumerable<RateTableInfo>> GetFiltered(string variant, string version, int? carrier, DateTime? validFrom, string culture)
        {
            IEnumerable<RateTable> tables =
                await m_DbContext.RateTables.Cast<RateTable>()
                    .Where(r => (string.IsNullOrEmpty(variant) || r.Variant == variant) &&
                                (string.IsNullOrEmpty(version) || r.VersionNumber == version) &&
                                (!carrier.HasValue || r.CarrierId == carrier.Value) &&
                                (!validFrom.HasValue || r.ValidFrom.CompareTo(validFrom.Value) >= 0) &&
                                (string.IsNullOrEmpty(culture) || r.Culture == culture)).ToListAsync();
            return tables.Select(r => r.ToRateTableInfo());
        }

        public async Task<RateTableInfo> AddNew(RateTableInfo newInfo)
        {
            RateTable t = RateTable.New(newInfo);
            m_DbContext.RateTables.Add(t);
            await m_DbContext.SaveChangesAsync();
            return t.ToRateTableInfo();
        }

        public async Task<bool> DeleteRateTable(int id)
        {
            RateTable table = await m_DbContext.RateTables.FirstOrDefaultAsync(r => r.Id == id);
            if (null != table)
            {
                m_DbContext.RateTables.Remove(table);
                await m_DbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
        #endregion
    }
}

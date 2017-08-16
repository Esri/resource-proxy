using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FP.Cloud.OnlineRateTable.Common.RateTable;

namespace FP.Cloud.OnlineRateTable.Common.Interfaces
{
    public interface IRateTableManager
    {
        /// <summary>
        /// Adds a new RateTable to the database
        /// </summary>
        /// <param name="newInfo">The new information.</param>
        /// <returns></returns>
        Task<RateTableInfo> AddNew(RateTableInfo newInfo);

        /// <summary>
        /// updates an existing rate table info in the database
        /// </summary>
        /// <param name="updatedInfo">The updated information.</param>
        /// <returns></returns>
        Task<RateTableInfo> Update(RateTableInfo updatedInfo);

        /// <summary>
        /// deletes an rate table database entry
        /// </summary>
        /// <param name="id">id of the rate table to be deleted</param>
        /// <returns></returns>
        Task<bool> DeleteRateTable(int id);

        /// <summary>
        /// Returns all RateTables from the database
        /// </summary>
        /// <param name="includeFileData">if set to true the Package file data is appended (long transmission time)</param>
        /// <returns></returns>
        Task<IEnumerable<RateTableInfo>> GetAll(bool includeFileData);

        /// <summary>
        /// returns a list of rate tables from the database with only the currently active table for each variant
        /// </summary>
        /// <param name="includeFileData">if set to true the Package file data is appended (long transmission time)</param>
        /// <param name="currentUtc">if a value is specified only rate tables with a valid from
        /// date lower than the specified one are returned</param>
        /// <returns></returns>
        Task<IEnumerable<RateTableInfo>> GetLatestForVariant(bool includeFileData, DateTime? currentUtc);

        /// <summary>
        /// returns a rate table specified by database id
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<RateTableInfo> GetById(int id);

        /// <summary>
        /// returns rate tables from the database using the filters provided. this method also includes paging mechanisms
        /// </summary>
        /// <param name="variant">the variant of the rate tables - if set to null all variants are returned</param>
        /// <param name="version">the specific version of the rate tables - if set to null all versions are returned</param>
        /// <param name="carrier">the carrier of the rate tables - if set to null all carriers are returned</param>
        /// <param name="minValidFrom">minimum valid from date - if set to null the lower boundary of the date range is ignored</param>
        /// <param name="maxValidFrom">maximum valid from date - if set to null the upper boundary of the date range is ignored</param>
        /// <param name="culture">the culture of the rate tables - if set to null all cultures are returned</param>
        /// <param name="start">the paging start index</param>
        /// <param name="count">the number of entries to be returned - if set to 0 all entries are being returned</param>
        /// <param name="includeFileData">if set to true the Package file data is appended (long transmission time)</param>
        /// <returns></returns>
        Task<IEnumerable<RateTableInfo>> GetFiltered(string variant, string version, int? carrier, DateTime? minValidFrom, DateTime? maxValidFrom, string culture, int start, int count, bool includeFileData);
    }
}
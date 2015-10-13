using System;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    public class EnvironmentInfo
    {
        public string Iso3CountryCode { get; set; }
        public int CarrierId { get; set; }
        public DateTime UtcDate { get; set; }
        public string Culture { get; set; }
        public string SenderZipCode { get; set; }
    }
}

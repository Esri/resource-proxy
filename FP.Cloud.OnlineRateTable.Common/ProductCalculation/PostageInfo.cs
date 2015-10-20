﻿using System;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    public class PostageInfo
    {
        public UInt64 PostageValue { get; set; }
        public ushort PostageDecimals { get; set; }
        public string CurrencySymbol { get; set; }
    }
}

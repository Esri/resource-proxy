using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FP.Cloud.OnlineRateTable.Web.Formatter
{
    public enum EParsingState
    {
        NoValue = 0,
        ValStarted,
        WidthStarted,
        PrecisionStarted,
        PrecisionFinished
    };
}
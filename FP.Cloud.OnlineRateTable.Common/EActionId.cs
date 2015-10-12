using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP.Cloud.OnlineRateTable.Common
{
    public enum EActionId
    {
        Finish = 1,
        ShowMenu = 2,
        Display = 3,
        RequestValue = 5,
        SelectIndex = 7,
        SelectValue = 8,
        NoProduct = 11,
        Continue = 12,
        TestImprint = 13,
        NoAction = 0,
        ManualPostage = 14,
        RequestString = 15,
        Unknown = 100,
    }
}

using FP.Cloud.OnlineRateTable.BusinessLogic;
using FP.Cloud.OnlineRateTable.Common.Interfaces;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP.Cloud.OnlineRateTable.Bootstrapper
{
    public class OrtModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IRateCalculation>().To<OnlineRateCalculation>();
            Bind<IRateTableManager>().To<RateTableManager>();
        }
    }
}

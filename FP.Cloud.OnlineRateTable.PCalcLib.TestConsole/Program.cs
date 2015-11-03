using System.IO;
using FP.Cloud.OnlineRateTable.Common.ProductCalculation;

namespace FP.Cloud.OnlineRateTable.PCalcLib.TestConsole
{
    internal class Program
    {
        #region Static Fields

        private static readonly FileInfo AMX_FILE = new FileInfo("Pt2097152.amx");
        private static readonly EnvironmentInfo ENVIRONMENT = new EnvironmentInfo { Culture = "deDE", SenderZipCode = "121" };
        private static readonly FileInfo TABLE_FILE = new FileInfo("Pt2097152.bin");
        private static readonly WeightInfo WEIGHT = new WeightInfo { WeightUnit = EWeightUnit.TenthGram, WeightValue = 200 };

        #endregion

        #region Methods

        private static void Main(string[] args)
        {
            int iterations = 10000;

            for (int i = 0; i < iterations; i++)
            {
                using (PCalcProxyContext context = new PCalcProxyContext(ENVIRONMENT, AMX_FILE.FullName, TABLE_FILE.FullName))
                {
                    context.Proxy.Start(ENVIRONMENT, WEIGHT);
                }
            }

            using (PCalcProxyContext context = new PCalcProxyContext(ENVIRONMENT, AMX_FILE.FullName, TABLE_FILE.FullName))
            {
                for (int i = 0; i < iterations; i++)
                {
                    context.Proxy.Start(ENVIRONMENT, WEIGHT);
                }
            }
        }

        #endregion
    }
}
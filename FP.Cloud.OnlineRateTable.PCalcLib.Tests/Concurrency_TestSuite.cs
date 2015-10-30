using FP.Cloud.OnlineRateTable.Common.ProductCalculation;
using Ninject;
using NUnit.Framework;

namespace FP.Cloud.OnlineRateTable.PCalcLib.Tests
{
    // ReSharper disable once InconsistentNaming
    [TestFixture]
    public class Concurrency_TestSuite
    {
        #region Public Methods and Operators

        [Test]
        public void ShouldBlockCalculationFromOtherThreads()
        {
            IKernel kernel = new StandardKernel();
            FP.Cloud.OnlineRateTable.PCalcLib.ICalculationResultProcessorProxy calcResult;
        }

        #endregion
    }
}
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using FP.Cloud.OnlineRateTable.Common.ProductCalculation;
using JetBrains.dotMemoryUnit;
using Ninject;
using NUnit.Framework;

namespace FP.Cloud.OnlineRateTable.PCalcLib.Tests
{
    // ReSharper disable once InconsistentNaming
    [TestFixture]
    public class Concurrency_TestSuite
    {
        private readonly Stopwatch m_Watch = new Stopwatch();

        [SetUp]
        public void SetUp()
        {
            m_Watch.Reset();
            m_Watch.Start();
        }

        #region Public Methods and Operators

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [DotMemoryUnit(FailIfRunWithoutSupport = false)]
        public void ShouldRunConcurrencyWithoutErrors(int iterations)
        {                                    
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < iterations; i++)
            {
                tasks.Add(new Task(RunStart));                
            }

            tasks.ForEach(x => x.Start());

            Task.WaitAll(tasks.ToArray());
            Pass();
        }

        private void RunStart()
        {
            EnvironmentInfo environment = new EnvironmentInfo { Culture = "deDE", SenderZipCode = "121" };
            WeightInfo weight = new WeightInfo { WeightUnit = EWeightUnit.TenthGram, WeightValue = 200 };
            Assert.That(new FileInfo("Pt2097152.amx").Exists, Is.True);
            Assert.That(new FileInfo("Pt2097152.bin").Exists, Is.True);

            using (var context = new PCalcProxyContext(new FileInfo("Pt2097152.amx").FullName, new FileInfo("Pt2097152.bin").FullName))
            {
                Assert.That(context.Proxy, Is.Not.Null);

                IPCalcProxy proxy = context.Proxy;
                proxy.Start(environment, weight);
            }
        }

        private void Pass()
        {
            m_Watch.Stop();
            var context = TestContext.CurrentContext;
            if (context.Result.State == TestState.Inconclusive)
            {
                Assert.Pass($"{m_Watch.Elapsed.TotalMilliseconds} ms");
                //Assert.Pass($"Elapsed runtime {m_Watch.Elapsed.TotalMilliseconds} ms, Max expected runtime {m_ExpectedMaximum.TotalMilliseconds} ms. Product: { string.Join(", ",info.ProductDescription.ReadyModeSelection)}");
            }
        }

        #endregion
    }
}
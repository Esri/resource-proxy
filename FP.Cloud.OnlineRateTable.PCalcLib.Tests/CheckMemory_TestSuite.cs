using System;
using System.Diagnostics;
using System.IO;
using FP.Cloud.OnlineRateTable.Common.ProductCalculation;
using JetBrains.dotMemoryUnit;
using NUnit.Framework;

namespace FP.Cloud.OnlineRateTable.PCalcLib.Tests
{
    // ReSharper disable once InconsistentNaming
    [TestFixture]
    public class CheckMemory_TestSuite
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
        [TestCase(1000)]
        [DotMemoryUnit(FailIfRunWithoutSupport = false)]
        public void ShouldReleaseAllResources(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                RunStart();
            }

            Pass();

            dotMemory.Check(memory => { Assert.That(memory.GetObjects(x => x.Type.Is<PCalcProxyContext>()).SizeInBytes, Is.EqualTo(0)); });
            dotMemory.Check(memory => { Assert.That(memory.GetObjects(x => x.Interface.Is<IPCalcProxy>()).SizeInBytes, Is.EqualTo(0)); });
        }

        private void RunStart()
        {
            EnvironmentInfo environment = new EnvironmentInfo { Culture = "deDE", SenderZipCode = "121" };
            WeightInfo weight = new WeightInfo { WeightUnit = EWeightUnit.TenthGram, WeightValue = 200 };
            Assert.That(new FileInfo("Pt2097152.amx").Exists, Is.True);
            Assert.That(new FileInfo("Pt2097152.bin").Exists, Is.True);

            using (var context = new PCalcProxyContext(environment, new FileInfo("Pt2097152.amx").FullName, new FileInfo("Pt2097152.bin").FullName))
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
            }
        }

        #endregion
    }
}
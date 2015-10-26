using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FP.Cloud.OnlineRateTable.Common.ProductCalculation;
using NUnit.Framework;

namespace FP.Cloud.OnlineRateTable.PCalcLib.Tests
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public class PCalcProxy_TestSuite
    {
        #region Constants

        private const int MAX_STEPS = 30;

        #endregion

        #region Fields

        private readonly Stopwatch m_Watch = new Stopwatch();

        private PCalcProxyContext m_Context;
        private EnvironmentInfo m_Environment;
        private TimeSpan m_ExpectedMaximum;
        private WeightInfo m_Weight;

        #endregion

        #region Public Methods and Operators

        [SetUp]
        public void SetUp()
        {
            FileInfo amxFile = new FileInfo("Pt2097152.amx");
            FileInfo tableFile = new FileInfo("Pt2097152.bin");

            Assert.IsTrue(amxFile.Exists);
            Assert.IsTrue(tableFile.Exists);

            m_Environment = new EnvironmentInfo { Culture = "deDE", SenderZipCode = "121" };
            m_Weight = new WeightInfo { WeightUnit = EWeightUnit.TenthGram, WeightValue = 200 };

            m_ExpectedMaximum = TimeSpan.FromMilliseconds(1000);
            m_Watch.Reset();
            m_Watch.Start();
            m_Context = new PCalcProxyContext(amxFile.FullName, tableFile.FullName);
        }

        [TestCase(1000)]
        [TestCase(100)]
        [TestCase(10)]
        public void ShouldCalculateCompleteProduct(int milliseconds)
        {
            m_ExpectedMaximum = TimeSpan.FromMilliseconds(milliseconds);

            Assert.IsNotNull(m_Context.Proxy);
            IPCalcProxy proxy = m_Context.Proxy;

            PCalcResultInfo result = proxy.Calculate(m_Environment, m_Weight);

            int steps = 0;

            for (int i = 0; i < MAX_STEPS; i++)
            {
                steps++;
                switch (result.QueryType)
                {
                    case EQueryType.ShowMenu:
                        var actionResult = new ActionResultInfo { Action = EActionId.ShowMenu, Label = 0, Results = new List<AnyInfo> { new AnyInfo { AnyValue = "0", AnyType = EAnyType.UINT32 } } };
                        result = proxy.Calculate(m_Environment, result.ProductDescription, actionResult);
                        break;

                    case EQueryType.None:
                        i = MAX_STEPS - 1;
                        break;

                    default:
                        Assert.Fail();
                        break;
                }
            }

            Assert.IsTrue(steps < MAX_STEPS);
            Assert.IsTrue(result.ProductDescription.State == EProductDescriptionState.Complete);
        }

        [TestCase(1000)]
        [TestCase(100)]
        [TestCase(10)]
        public void ShouldCalculateFirstStep(int milliseconds)
        {
            m_ExpectedMaximum = TimeSpan.FromMilliseconds(milliseconds);

            Assert.IsNotNull(m_Context.Proxy);
            IPCalcProxy proxy = m_Context.Proxy;

            PCalcResultInfo result = proxy.Calculate(m_Environment, m_Weight);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.QueryType == EQueryType.ShowMenu);
            Assert.IsTrue(result.ProductDescription.Weight.WeightValue != 0, "Product has no weight");
        }

        [TestCase(1000)]
        [TestCase(100)]
        [TestCase(10)]
        public void ShouldCalculateNextStep(int milliseconds)
        {
            m_ExpectedMaximum = TimeSpan.FromMilliseconds(milliseconds);

            Assert.IsNotNull(m_Context.Proxy);
            IPCalcProxy proxy = m_Context.Proxy;

            //first step - we need an product description
            PCalcResultInfo result = proxy.Calculate(m_Environment, m_Weight);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.QueryType == EQueryType.ShowMenu);

            // second step
            var actionResult = new ActionResultInfo { Action = EActionId.ShowMenu, Label = 0, Results = new List<AnyInfo> { new AnyInfo { AnyValue = "0", AnyType = EAnyType.UINT32 } } };
            result = proxy.Calculate(m_Environment, result.ProductDescription, actionResult);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.QueryType == EQueryType.ShowMenu);
            Assert.IsTrue(result.ProductDescription.Weight.WeightValue != 0, "Product has no weight");
        }

        [TearDown]
        public void TearDown()
        {
            m_Context.Dispose();
            m_Context = null;

            m_Watch.Stop();
            var context = TestContext.CurrentContext;
            if (context.Result.State == TestState.Success)
            {
                if (m_Watch.Elapsed > m_ExpectedMaximum)
                {
                    Assert.Ignore($"Elapsed runtime {m_Watch.Elapsed.TotalMilliseconds} ms, Max expected runtime {m_ExpectedMaximum.TotalMilliseconds} ms");
                }
            }
        }

        #endregion
    }
}
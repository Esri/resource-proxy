using System.Collections.Generic;
using System.IO;
using FP.Cloud.OnlineRateTable.Common.ProductCalculation;
using NUnit.Framework;

namespace FP.Cloud.OnlineRateTable.PCalcLib.Tests
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public class PCalcProxy_TestSuite
    {
        #region Fields

        private PCalcProxyContext m_Context;
        private EnvironmentInfo m_Environment;
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

            m_Context = new PCalcProxyContext(amxFile.FullName, tableFile.FullName);
            m_Environment = new EnvironmentInfo { Culture = "de", SenderZipCode = "123" };
            m_Weight = new WeightInfo { WeightUnit = EWeightUnit.TenthGram, WeightValue = 200 };
        }

        [Test]
        public void ShouldCalculateFirstStep()
        {
            Assert.IsNotNull(m_Context.Proxy);
            IPCalcProxy proxy = m_Context.Proxy;

            PCalcResultInfo result = proxy.Calculate(m_Environment, m_Weight);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.QueryType == EQueryType.ShowMenu);
        }

        [Test]
        public void ShouldCalculateNextStep()
        {
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
        }

        [TearDown]
        public void TearDown()
        {
            m_Context.Dispose();
            m_Context = null;
        }

        #endregion
    }
}
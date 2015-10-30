using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private EnvironmentInfo m_Environment;
        private TimeSpan m_ExpectedMaximum;
        private WeightInfo m_Weight;
        private readonly FileInfo m_AmxFile = new FileInfo("Pt2097152.amx");
        private readonly FileInfo m_TableFile = new FileInfo("Pt2097152.bin");

        #endregion

        #region Public Methods and Operators

        [SetUp]
        public void SetUp()
        {

            Assert.IsTrue(m_AmxFile.Exists);
            Assert.IsTrue(m_TableFile.Exists);

            m_Environment = new EnvironmentInfo { Culture = "deDE", SenderZipCode = "121" };
            m_Weight = new WeightInfo { WeightUnit = EWeightUnit.TenthGram, WeightValue = 200 };

            m_ExpectedMaximum = TimeSpan.FromMilliseconds(1000);
            m_Watch.Reset();
            m_Watch.Start();
        }

        [TestCase(1000)]
        [TestCase(100)]
        [TestCase(50)]
        [TestCase(10)]
        public void ShouldHandleCalculateCompleteProduct(int milliseconds)
        {
            m_ExpectedMaximum = TimeSpan.FromMilliseconds(milliseconds);

            PCalcResultInfo result = Start(m_Environment, m_Weight);

            int steps = 0;

            for (int i = 0; i < MAX_STEPS; i++)
            {
                if (result.ProductDescription.State == EProductDescriptionState.Complete)
                {
                    break;
                }

                steps++;
                switch (result.QueryType)
                {
                    case EQueryType.ShowMenu:
                        var actionResult = new ActionResultInfo { Action = EActionId.ShowMenu, Label = 0, Results = new List<AnyInfo> { new AnyInfo { AnyValue = (result.QueryDescription.Entries.Count -1).ToString(), AnyType = EAnyType.UINT32 } } };
                        result = Calculate(m_Environment, result.ProductDescription, actionResult);
                        break;

                    case EQueryType.None:
                        i = MAX_STEPS - 1;
                        break;

                    default:
                        Assert.Fail();
                        break;
                }

                Assert.IsNotNull(result.ProductDescription);
                Assert.IsTrue(result.ProductDescription.ProductId > 0);
            }

            Assert.IsTrue(steps < MAX_STEPS);
            Assert.IsNotNull(result.ProductDescription);
            Assert.IsNotNull(result.ProductDescription.Postage);

            Assert.IsTrue(result.ProductDescription.State == EProductDescriptionState.Complete);
            Assert.IsTrue(result.ProductDescription.Postage.PostageValue > 0);
            Assert.IsTrue(result.ProductDescription.ProductCode > 0);

            Pass(result);
        }

        [TestCase(1000)]
        [TestCase(100)]
        [TestCase(50)]
        [TestCase(10)]
        public void ShouldCalculateFirstStep(int milliseconds)
        {
            m_ExpectedMaximum = TimeSpan.FromMilliseconds(milliseconds);

            PCalcResultInfo result = Start(m_Environment, m_Weight);

            Assert.IsNotNull(result);
            Assert.AreEqual(EQueryType.ShowMenu, result.QueryType);
            Assert.Greater(result.ProductDescription.Weight.WeightValue, 0, "Product has no weight");

            Pass(result);
        }

        [TestCase(1000)]
        [TestCase(100)]
        [TestCase(50)]
        [TestCase(10)]
        public void ShouldCalculateNextStep(int milliseconds)
        {
            m_ExpectedMaximum = TimeSpan.FromMilliseconds(milliseconds);


            ProductDescriptionInfo product = new ProductDescriptionInfo { ProductId = 1, WeightClass = 1, Weight = m_Weight };

            var actionResult = new ActionResultInfo { Action = EActionId.ShowMenu, Label = 0, Results = new List<AnyInfo> { new AnyInfo { AnyValue = "0", AnyType = EAnyType.UINT32 } } };
            PCalcResultInfo result = Calculate(m_Environment, product, actionResult);

            Assert.IsNotNull(result);
            Assert.AreEqual(EQueryType.ShowMenu, result.QueryType);
            Assert.Greater(result.ProductDescription.Weight.WeightValue, 0, "Product has no weight");

            Pass(result);
        }

        [Test]
        public void ShouldHandleRequestValue()
        {

            var actionResult = new ActionResultInfo { Action = EActionId.ShowMenu, Label = 0, Results = new List<AnyInfo> { new AnyInfo { AnyValue = "0", AnyType = EAnyType.INT32 } } };
            PCalcResultInfo result = Start(m_Environment, new WeightInfo());

            result = Calculate(m_Environment, result.ProductDescription, actionResult);
            result = Calculate(m_Environment, result.ProductDescription, actionResult);
            result = Calculate(m_Environment, result.ProductDescription, actionResult);

            actionResult.Results.Single().AnyValue = "4";
            actionResult.Action = EActionId.ShowMenu;
            actionResult.Label = 1;

            Assert.DoesNotThrow( () => result = Calculate(m_Environment, result.ProductDescription, actionResult));
            Assert.AreEqual(EQueryType.RequestValue, result.QueryType);

            actionResult.Results.Clear();
            actionResult.Results.Add(new AnyInfo {AnyType = EAnyType.UINT32, AnyValue = "123456"});
            actionResult.Action = EActionId.RequestValue;
            actionResult.Label = result.QueryDescription.Label;

            Assert.DoesNotThrow(() => result = Calculate(m_Environment, result.ProductDescription, actionResult));

            Pass(result);
        }

        [Test]
        public void ShouldHandleDisplay()
        {

            var actionResult = new ActionResultInfo { Action = EActionId.ShowMenu, Label = 0, Results = new List<AnyInfo> { new AnyInfo { AnyValue = "0", AnyType = EAnyType.INT32 } } };
            PCalcResultInfo result = Start(m_Environment, m_Weight);

            result = Calculate(m_Environment, result.ProductDescription, actionResult);
            result = Calculate(m_Environment, result.ProductDescription, actionResult);
            result = Calculate(m_Environment, result.ProductDescription, actionResult);

            actionResult.Results.Single().AnyValue = "4";
            actionResult.Action = EActionId.ShowMenu;
            actionResult.Label = 1;

            Assert.DoesNotThrow(() => result = Calculate(m_Environment, result.ProductDescription, actionResult));
            Assert.AreEqual(EQueryType.RequestValue, result.QueryType);

            actionResult.Results.Clear();
            actionResult.Results.Add(new AnyInfo { AnyType = EAnyType.UINT32, AnyValue = "123456" });
            actionResult.Action = EActionId.RequestValue;
            actionResult.Label = result.QueryDescription.Label;

            Assert.DoesNotThrow(() => result = Calculate(m_Environment, result.ProductDescription, actionResult));
            Assert.AreEqual(EQueryType.ShowDisplay, result.QueryType);

            actionResult.Results.Clear();
            actionResult.Results = new List<AnyInfo>();
            actionResult.Label = 1;
            actionResult.Action = EActionId.Display;            

            Assert.DoesNotThrow(() => result = Calculate(m_Environment, result.ProductDescription, actionResult));

            actionResult.Results.Clear();
            actionResult.Results.Add(new AnyInfo { AnyType = EAnyType.UINT32, AnyValue = "15528" });
            actionResult.Action = EActionId.RequestValue;
            actionResult.Label = result.QueryDescription.Label;

            Assert.DoesNotThrow(() => result = Calculate(m_Environment, result.ProductDescription, actionResult));

            Pass(result);
        }

        [Test]
        public void ShouldHandleStepBackToPreviousProduct()
        {

            var actionResult = new ActionResultInfo { Action = EActionId.ShowMenu, Label = 0, Results = new List<AnyInfo> { new AnyInfo { AnyValue = "0", AnyType = EAnyType.UINT32 } } };
            PCalcResultInfo result = Start(m_Environment, new WeightInfo());

            result = Calculate(m_Environment, result.ProductDescription, actionResult);
            result = Calculate(m_Environment, result.ProductDescription, actionResult);

            int productID = result.ProductDescription.ProductId;

            result = Calculate(m_Environment, result.ProductDescription, actionResult);
            Assert.AreNotEqual(productID, result.ProductDescription.ProductId);

            result = Back(m_Environment, result.ProductDescription);
            Assert.AreEqual(productID, result.ProductDescription.ProductId);

            Pass(result);
        }

        private void Pass(PCalcResultInfo info)
        {
            m_Watch.Stop();
            var context = TestContext.CurrentContext;
            if (context.Result.State == TestState.Inconclusive)
            {
                Assert.Pass($"{m_Watch.Elapsed.TotalMilliseconds} ms");
                //Assert.Pass($"Elapsed runtime {m_Watch.Elapsed.TotalMilliseconds} ms, Max expected runtime {m_ExpectedMaximum.TotalMilliseconds} ms. Product: { string.Join(", ",info.ProductDescription.ReadyModeSelection)}");
            }
        }

        private PCalcResultInfo Calculate(EnvironmentInfo info, ProductDescriptionInfo product, ActionResultInfo actionResult)
        {
            using (var context = new PCalcProxyContext(m_AmxFile.FullName, m_TableFile.FullName))
            {
                IPCalcProxy proxy = context.Proxy;
                Assert.That(proxy, Is.Not.Null);

                return proxy.Calculate(m_Environment, product, actionResult);
            }
        }

        private PCalcResultInfo Start(EnvironmentInfo info, WeightInfo weight)
        {
            using (var context = new PCalcProxyContext(m_AmxFile.FullName, m_TableFile.FullName))
            {
                IPCalcProxy proxy = context.Proxy;
                Assert.That(proxy, Is.Not.Null);

                return proxy.Start(m_Environment, weight);
            }
        }

        private PCalcResultInfo Back(EnvironmentInfo info, ProductDescriptionInfo product)
        {
            using (var context = new PCalcProxyContext(m_AmxFile.FullName, m_TableFile.FullName))
            {
                IPCalcProxy proxy = context.Proxy;
                Assert.That(proxy, Is.Not.Null);

                return proxy.Back(m_Environment, product);
            }
        }

        #endregion
    }
}
using System;
using System.IO;
using FP.Cloud.OnlineRateTable.Common.ProductCalculation;
using NUnit.Framework;

namespace FP.Cloud.OnlineRateTable.PCalcLib.Tests
{
    // ReSharper disable once InconsistentNaming
    [TestFixture]
    public class PCalcProxyContext_TestSuite
    {
        private EnvironmentInfo m_Environment;
        private int m_TestCount = 0;

        [SetUp]
        public void SetUp()
        {
            m_TestCount++;
            m_Environment = new EnvironmentInfo() { Culture = "de", UtcDate = DateTime.Now.AddDays(m_TestCount), SenderZipCode = "123" };
        }

        #region Public Methods and Operators

        [Test]
        public void ShouldCreateContextIfValidContentGiven()
        {
            FileInfo amxFile = new FileInfo("Pt2097152.amx");
            FileInfo tableFile = new FileInfo("Pt2097152.bin");

            Assert.IsTrue(amxFile.Exists);
            Assert.IsTrue(tableFile.Exists);

            using (var context = new PCalcProxyContext(m_Environment, amxFile.FullName, tableFile.FullName))
            {
                Assert.IsNotNull(context.Proxy);
            }
        }

        [Test]
        public void ShouldThrowPCalcLibErrorCodeExceptionIfAmxFileLikeTableFile()
        {
            FileInfo amxFile = new FileInfo("Pt2097152.bin");
            FileInfo tableFile = new FileInfo("Pt2097152.bin");

            Assert.IsTrue(amxFile.Exists);
            Assert.IsTrue(tableFile.Exists);

            using (var context = new PCalcProxyContext(m_Environment, amxFile.FullName, tableFile.FullName))
            {
                Assert.Throws<PCalcLibErrorCodeException>(() => { var proxy = context.Proxy; });
            }
        }

        [Test]
        public void ShouldThrowPCalcLibErrorCodeExceptionIfContentFileDoesNotExist()
        {
            FileInfo amxFile = new FileInfo("Pt2097152.amx2");
            FileInfo tableFile = new FileInfo("Pt2097152.bin2");

            using (var context = new PCalcProxyContext(m_Environment, amxFile.FullName, tableFile.FullName))
            {
                Assert.Throws<PCalcLibErrorCodeException>(() => { var proxy = context.Proxy; });
            }
        }

        [Test]
        public void ShouldThrowPCalcLibErrorCodeExceptionIfEmptyContentFile()
        {
            using (var context = new PCalcProxyContext(m_Environment, string.Empty, string.Empty))
            {
                Assert.Throws<PCalcLibErrorCodeException>(() => { var proxy = context.Proxy; });
            }
        }

        [Test]
        public void ShouldThrowPCalcLibErrorCodeExceptionIfNullContentFile()
        {
            using (var context = new PCalcProxyContext(m_Environment, null, null))
            {
                Assert.Throws<PCalcLibErrorCodeException>(() => { var proxy = context.Proxy; });
            }
        }

        [Test]
        public void ShouldThrowPCalcLibExceptionIfTableFileLikeAmxFile()
        {
            FileInfo amxFile = new FileInfo("Pt2097152.amx");
            FileInfo tableFile = new FileInfo("Pt2097152.amx");

            Assert.IsTrue(amxFile.Exists);
            Assert.IsTrue(tableFile.Exists);

            using (var context = new PCalcProxyContext(m_Environment, amxFile.FullName, tableFile.FullName))
            {
                Assert.Throws<PCalcLibException>(() => { var proxy = context.Proxy; });
            }
        }

        #endregion
    }
}
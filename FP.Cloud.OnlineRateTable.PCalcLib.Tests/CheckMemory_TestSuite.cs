using System;
using System.IO;
using JetBrains.dotMemoryUnit;
using NUnit.Framework;

namespace FP.Cloud.OnlineRateTable.PCalcLib.Tests
{
    // ReSharper disable once InconsistentNaming
    [TestFixture]
    public class CheckMemory_TestSuite
    {
        #region Public Methods and Operators

        [Test]
        [DotMemoryUnit(FailIfRunWithoutSupport = false)]
        public void ShouldReleaseAllResources()
        {
            CreateContext();
            CreateContext();
            CreateContext();
          
            dotMemory.Check(memory => { Assert.That(memory.GetObjects(x => x.Type.Is<PCalcProxyContext>()).SizeInBytes, Is.EqualTo(0)); });
            dotMemory.Check(memory => { Assert.That(memory.GetObjects(x => x.Interface.Is<IPCalcProxy>()).SizeInBytes, Is.EqualTo(0)); });
        }

        private void CreateContext()
        {
            FileInfo amxFile = new FileInfo("Pt2097152.amx");
            FileInfo tableFile = new FileInfo("Pt2097152.bin");

            Assert.That(amxFile.Exists, Is.True);
            Assert.That(tableFile.Exists, Is.True);

            using (var context = new PCalcProxyContext(amxFile.FullName, tableFile.FullName))
            {
                Assert.That(context.Proxy, Is.Not.Null);
            }
        }

        #endregion
    }
}
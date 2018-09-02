using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HidWizards.UCR.Core.Utilities;
using NUnit.Framework;

namespace HidWizards.UCR.Tests.HelperTests
{
    [TestFixture]
    public class FunctionTests
    {
        [Test]
        public void InvertTest()
        {
            Assert.AreEqual(Functions.Invert(0), 0);
            Assert.AreEqual(Functions.Invert(32767), -32768);
            Assert.AreEqual(Functions.Invert(-32768), 32767);
            Assert.AreEqual(Functions.Invert(1), -1);
            Assert.AreEqual(Functions.Invert(-1), 1);
        }
    }
}

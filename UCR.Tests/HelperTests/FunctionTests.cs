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
            Assert.AreEqual(Functions.Invert(Constants.AxisMaxValue), Constants.AxisMinValue);
            Assert.AreEqual(Functions.Invert(Constants.AxisMinValue), Constants.AxisMaxValue);
            Assert.AreEqual(Functions.Invert(1), -1);
            Assert.AreEqual(Functions.Invert(-1), 1);
        }

        [Test]
        public void ClampTest()
        {
            Assert.AreEqual(Functions.ClampAxisRange(Constants.AxisMinValue - 1), Constants.AxisMinValue);
            Assert.AreEqual(Functions.ClampAxisRange(Constants.AxisMaxValue + 1), Constants.AxisMaxValue);
            Assert.AreEqual(Functions.ClampAxisRange(Constants.AxisMaxValue), Constants.AxisMaxValue);
            Assert.AreEqual(Functions.ClampAxisRange(Constants.AxisMinValue), Constants.AxisMinValue);
            Assert.AreEqual(Functions.ClampAxisRange(0), 0);
            Assert.AreEqual(Functions.ClampAxisRange(1), 1);
            Assert.AreEqual(Functions.ClampAxisRange(-1), -1);
        }
    }
}

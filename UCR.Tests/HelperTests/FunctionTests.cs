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
            Assert.AreEqual(0, Functions.Invert(0));
            Assert.AreEqual(Constants.AxisMinValue, Functions.Invert(Constants.AxisMaxValue));
            Assert.AreEqual(Functions.Invert(Constants.AxisMinValue), Constants.AxisMaxValue);
            Assert.AreEqual(Functions.Invert(1), -1);
            Assert.AreEqual(Functions.Invert(-1), 1);
        }

        [Test]
        public void ClampTest()
        {
            Assert.AreEqual(Constants.AxisMinValue, Functions.ClampAxisRange(Constants.AxisMinValue - 1));
            Assert.AreEqual(Constants.AxisMaxValue, Functions.ClampAxisRange(Constants.AxisMaxValue + 1));
            Assert.AreEqual(Constants.AxisMaxValue, Functions.ClampAxisRange(Constants.AxisMaxValue));
            Assert.AreEqual(Constants.AxisMinValue, Functions.ClampAxisRange(Constants.AxisMinValue));
            Assert.AreEqual(0, Functions.ClampAxisRange(0));
            Assert.AreEqual(1, Functions.ClampAxisRange(1));
            Assert.AreEqual(-1, Functions.ClampAxisRange(-1));
        }
    }
}

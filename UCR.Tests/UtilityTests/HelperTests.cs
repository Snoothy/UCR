using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HidWizards.UCR.Core.Utilities;
using HidWizards.UCR.Core.Utilities.AxisHelpers;
using NUnit.Framework;

namespace HidWizards.UCR.Tests.HelperTests
{
    [TestFixture]
    public class HelperTests
    {
        [Test]
        public void DeadZoneHelperInitTest()
        {
            var helper = new DeadZoneHelper();
            // On initialize, the helper should work for DZ 0 without setting Percent
            Assert.AreEqual(Constants.AxisMaxValue, helper.ApplyRangeDeadZone(Constants.AxisMaxValue));
            Assert.AreEqual(Constants.AxisMinValue, helper.ApplyRangeDeadZone(Constants.AxisMinValue));
            Assert.AreEqual(0, helper.ApplyRangeDeadZone(0));
            Assert.AreEqual(1, helper.ApplyRangeDeadZone(1));
            Assert.AreEqual(-1, helper.ApplyRangeDeadZone(-1));
        }

        [Test]
        public void DeadZoneHelperValuesTest()
        {
            var helper = new DeadZoneHelper {Percentage = 50};
            // At 50% DZ, we should still be able to reach extremes
            var max = helper.ApplyRangeDeadZone(Constants.AxisMaxValue);
            //Assert.AreEqual(Constants.AxisMaxValue, max);
            var min = helper.ApplyRangeDeadZone(Constants.AxisMinValue);
            Assert.AreEqual(Constants.AxisMinValue, min);
            Assert.AreEqual(0, helper.ApplyRangeDeadZone(0));
        }

        [Test]
        public void SensitivityTest()
        {
            var helper = new SensitivityHelper();
            Assert.AreEqual(Constants.AxisMinValue, helper.ApplyRangeSensitivity(Constants.AxisMinValue));
            Assert.AreEqual(Constants.AxisMaxValue, helper.ApplyRangeSensitivity(Constants.AxisMaxValue));
            Assert.AreEqual(0, helper.ApplyRangeSensitivity(0));
            helper.Percentage = 50;
            Assert.AreEqual(Constants.AxisMinValue, helper.ApplyRangeSensitivity(Constants.AxisMinValue));
            Assert.AreEqual(Constants.AxisMaxValue, helper.ApplyRangeSensitivity(Constants.AxisMaxValue));
            helper.Percentage = 20;
            Assert.AreEqual(Constants.AxisMinValue, helper.ApplyRangeSensitivity(Constants.AxisMinValue));
            Assert.AreEqual(Constants.AxisMaxValue, helper.ApplyRangeSensitivity(Constants.AxisMaxValue));
            Assert.AreEqual(0, helper.ApplyRangeSensitivity(0));
        }

        [Test]
        public void CircularDeadZoneTest()
        {
            var helper = new CircularDeadZoneHelper();
            Assert.AreEqual(new long[] { Constants.AxisMaxValue, 0 }, helper.ApplyRangeDeadZone(new long[] { Constants.AxisMaxValue, 0 }));
            Assert.AreEqual(new long[] { Constants.AxisMinValue, 0 }, helper.ApplyRangeDeadZone(new long[] { Constants.AxisMinValue, 0 }));
            helper.Percentage = 50;
            Assert.AreEqual(new long[] {Constants.AxisMaxValue, 0}, helper.ApplyRangeDeadZone(new long[] { Constants.AxisMaxValue, 0 }));
            Assert.AreEqual(new long[] {Constants.AxisMinValue, 0}, helper.ApplyRangeDeadZone(new long[] { Constants.AxisMinValue, 0 }));
            helper.Percentage = 20;
            Assert.AreEqual(new long[] {Constants.AxisMaxValue, 0}, helper.ApplyRangeDeadZone(new long[] { Constants.AxisMaxValue, 0 }));
            Assert.AreEqual(new long[] {Constants.AxisMinValue, 0}, helper.ApplyRangeDeadZone(new long[] { Constants.AxisMinValue, 0 }));
        }
    }
}

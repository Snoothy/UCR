using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HidWizards.UCR.Core.Utilities;
using HidWizards.UCR.Core.Utilities.AxisHelpers;
using NUnit.Framework;

namespace HidWizards.UCR.Tests.UtilityTests.HelperTests
{
    [TestFixture]
    public class DeadZoneHelperTests
    {
        [TestCase(Constants.AxisMaxValue, ExpectedResult = Constants.AxisMaxValue, TestName = "DeadZoneHelper (Init): Max returns Max")]
        [TestCase(Constants.AxisMinValue, ExpectedResult = Constants.AxisMinValue, TestName = "DeadZoneHelper (Init): Min returns Min")]
        [TestCase(0, ExpectedResult = 0, TestName = "DeadZoneHelper (Init): 0 returns 0")]
        [TestCase(1, ExpectedResult = 1, TestName = "DeadZoneHelper (Init): 1 returns 1")]
        [TestCase(-1, ExpectedResult = -1, TestName = "DeadZoneHelper (Init): -1 returns -1")]
        public long DeadZoneHelperInitTests(short inputValue)
        {
            var helper = new DeadZoneHelper();
            return helper.ApplyRangeDeadZone(inputValue);
        }

        [TestCase(0, 50, ExpectedResult = 0, TestName = "DeadZoneHelper (50): 0 returns 0")]
        [TestCase(16384, 50, ExpectedResult = 1, TestName = "DeadZoneHelper (50): Half Positive deflection is outside DZ")]
        [TestCase(-16384, 50, ExpectedResult = -1, TestName = "DeadZoneHelper (50): Half Negative deflection is outside DZ")]
        [TestCase(16383, 50, ExpectedResult = 0, TestName = "DeadZoneHelper (50): Below Half Positive deflection is inside DZ")]
        [TestCase(-16383, 50, ExpectedResult = 0, TestName = "DeadZoneHelper (50): Below Half Negative deflection is inside DZ")]
        public short DeadZoneHelperValueTests(short inputValue, int percentage)
        {
            var helper = new DeadZoneHelper { Percentage = percentage };
            return helper.ApplyRangeDeadZone(inputValue);
        }

        [TestCase(Constants.AxisMaxValue, 10, ExpectedResult = Constants.AxisMaxValue, TestName = "DeadZoneHelper (10): Max returns Max")]
        [TestCase(Constants.AxisMaxValue, 20, ExpectedResult = Constants.AxisMaxValue, TestName = "DeadZoneHelper (20): Max returns Max")]
        [TestCase(Constants.AxisMaxValue, 30, ExpectedResult = Constants.AxisMaxValue, TestName = "DeadZoneHelper (30): Max returns Max")]
        [TestCase(Constants.AxisMaxValue, 40, ExpectedResult = Constants.AxisMaxValue, TestName = "DeadZoneHelper (40): Max returns Max")]
        [TestCase(Constants.AxisMaxValue, 50, ExpectedResult = Constants.AxisMaxValue, TestName = "DeadZoneHelper (50): Max returns Max")]
        [TestCase(Constants.AxisMaxValue, 60, ExpectedResult = Constants.AxisMaxValue, TestName = "DeadZoneHelper (60): Max returns Max")]
        [TestCase(Constants.AxisMaxValue, 70, ExpectedResult = Constants.AxisMaxValue, TestName = "DeadZoneHelper (70): Max returns Max")]
        [TestCase(Constants.AxisMaxValue, 80, ExpectedResult = Constants.AxisMaxValue, TestName = "DeadZoneHelper (80): Max returns Max")]
        [TestCase(Constants.AxisMaxValue, 90, ExpectedResult = Constants.AxisMaxValue, TestName = "DeadZoneHelper (90): Max returns Max")]

        [TestCase(Constants.AxisMinValue, 10, ExpectedResult = Constants.AxisMinValue, TestName = "DeadZoneHelper (10): Min returns Min")]
        [TestCase(Constants.AxisMinValue, 20, ExpectedResult = Constants.AxisMinValue, TestName = "DeadZoneHelper (20): Min returns Min")]
        [TestCase(Constants.AxisMinValue, 30, ExpectedResult = Constants.AxisMinValue, TestName = "DeadZoneHelper (30): Min returns Min")]
        [TestCase(Constants.AxisMinValue, 40, ExpectedResult = Constants.AxisMinValue, TestName = "DeadZoneHelper (40): Min returns Min")]
        [TestCase(Constants.AxisMinValue, 50, ExpectedResult = Constants.AxisMinValue, TestName = "DeadZoneHelper (50): Min returns Min")]
        [TestCase(Constants.AxisMinValue, 60, ExpectedResult = Constants.AxisMinValue, TestName = "DeadZoneHelper (60): Min returns Min")]
        [TestCase(Constants.AxisMinValue, 70, ExpectedResult = Constants.AxisMinValue, TestName = "DeadZoneHelper (70): Min returns Min")]
        [TestCase(Constants.AxisMinValue, 80, ExpectedResult = Constants.AxisMinValue, TestName = "DeadZoneHelper (80): Min returns Min")]
        [TestCase(Constants.AxisMinValue, 90, ExpectedResult = Constants.AxisMinValue, TestName = "DeadZoneHelper (90): Min returns Min")]
        public long DeadZoneHelperMaxTests(short inputValue, int percentage)
        {
            var helper = new DeadZoneHelper { Percentage = percentage };
            return helper.ApplyRangeDeadZone(inputValue);
        }

        [TestCase(101, ExpectedResult = 100, TestName = "DeadZoneHelper (101): Percentages over 100 should clamp to 100")]
        [TestCase(-1, ExpectedResult = 0, TestName = "DeadZoneHelper (-1): Percentages below 0 should clamp to 0")]
        public long DeadZoneValidationTest(int percentage)
        {
            var helper = new DeadZoneHelper();
            helper.Percentage = percentage;
            return helper.Percentage;
        }
    }
}

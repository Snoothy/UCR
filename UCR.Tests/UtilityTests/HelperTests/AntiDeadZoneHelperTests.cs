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
    public class AntiDeadZoneHelperTests
    {
        [TestCase(Constants.AxisMaxValue, ExpectedResult = Constants.AxisMaxValue, TestName = "AntiDeadZoneHelper (Init): Max returns Max")]
        [TestCase(Constants.AxisMinValue, ExpectedResult = Constants.AxisMinValue, TestName = "AntiDeadZoneHelper (Init): Min returns Min")]
        [TestCase(0, ExpectedResult = 0, TestName = "AntiDeadZoneHelper (Init): 0 returns 0")]
        [TestCase(1, ExpectedResult = 1, TestName = "AntiDeadZoneHelper (Init): 1 returns 1")]
        [TestCase(-1, ExpectedResult = -1, TestName = "AntiDeadZoneHelper (Init): -1 returns -1")]
        public long AntiDeadZoneHelperInitTests(short inputValue)
        {
            var helper = new AntiDeadZoneHelper();
            return helper.ApplyRangeAntiDeadZone(inputValue);
        }

        [TestCase(0, 50, ExpectedResult = 0, TestName = "AntiDeadZoneHelper (50): 0 returns 0")]
        [TestCase(16384, 25, ExpectedResult = 20480, TestName = "AntiDeadZoneHelper (50): Half Positive deflection adds 25%")]
        [TestCase(16384, 50, ExpectedResult = 24576, TestName = "AntiDeadZoneHelper (50): Half Positive deflection adds 50%")]
        [TestCase(16384, 75, ExpectedResult = 28671, TestName = "AntiDeadZoneHelper (50): Half Positive deflection adds 75%")]
        [TestCase(-16384, 25, ExpectedResult = -20480, TestName = "AntiDeadZoneHelper (50): Half Negative deflection adds 25%")]
        [TestCase(-16384, 50, ExpectedResult = -24576, TestName = "AntiDeadZoneHelper (50): Half Negative deflection adds 50%")]
        [TestCase(-16384, 75, ExpectedResult = -28671, TestName = "AntiDeadZoneHelper (50): Half Negative deflection adds 75%")]
        public short AntiDeadZoneHelperValueTests(short inputValue, int percentage)
        {
            var helper = new AntiDeadZoneHelper { Percentage = percentage };
            return helper.ApplyRangeAntiDeadZone(inputValue);
        }

        [TestCase(101, ExpectedResult = 100, TestName = "AntiDeadZoneHelper (101): Percentages over 100 should clamp to 100")]
        [TestCase(-1, ExpectedResult = 0, TestName = "AntiDeadZoneHelper (-1): Percentages below 0 should clamp to 0")]
        public long DeadZoneValidationTest(int percentage)
        {
            var helper = new AntiDeadZoneHelper();
            helper.Percentage = percentage;
            return helper.Percentage;
        }
    }
}

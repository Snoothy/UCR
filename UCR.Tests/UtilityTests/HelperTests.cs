using HidWizards.UCR.Core.Utilities;
using HidWizards.UCR.Core.Utilities.AxisHelpers;
using NUnit.Framework;

namespace HidWizards.UCR.Tests.UtilityTests
{
    [TestFixture]
    public class HelperTests
    {
        [TestCase(Constants.AxisMaxValue, ExpectedResult = Constants.AxisMaxValue, TestName = "DeadZoneHelper (Init): Max returns Max")]
        [TestCase(Constants.AxisMinValue, ExpectedResult = Constants.AxisMinValue, TestName = "DeadZoneHelper (Init): Min returns Min")]
        [TestCase(0, ExpectedResult = 0, TestName = "DeadZoneHelper (Init): 0 returns 0")]
        [TestCase(1, ExpectedResult = 1, TestName = "DeadZoneHelper (Init): 1 returns 1")]
        [TestCase(-1, ExpectedResult = -1, TestName = "DeadZoneHelper (Init): -1 returns -1")]
        public long DeadZoneHelperInitTests(long inputValue)
        {
            var helper = new DeadZoneHelper();
            return helper.ApplyRangeDeadZone(inputValue);
        }

        [TestCase(Constants.AxisMaxValue, 50, ExpectedResult = Constants.AxisMaxValue, TestName = "DeadZoneHelper (50): Max returns Max")]
        [TestCase(Constants.AxisMinValue, 50, ExpectedResult = Constants.AxisMinValue, TestName = "DeadZoneHelper (50): Min returns Min")]
        [TestCase(0, 50, ExpectedResult = 0, TestName = "DeadZoneHelper (50): 0 returns 0")]
        [TestCase(16384, 50, ExpectedResult = 1, TestName = "DeadZoneHelper (50): Positive values above 16383 are outside DZ")]
        [TestCase(-16384, 50, ExpectedResult = -1, TestName = "DeadZoneHelper (50): Negative values below 16383 are outside DZ")]
        [TestCase(16383, 50, ExpectedResult = 0, TestName = "DeadZoneHelper (50): Positive values below 16384 are inside DZ")]
        [TestCase(-16383, 50, ExpectedResult = 0, TestName = "DeadZoneHelper (50): Negative values below 16384 are inside DZ")]
        public long DeadZoneHelperValueTests(long inputValue, int percentage)
        {
            var helper = new DeadZoneHelper {Percentage = percentage};
            return helper.ApplyRangeDeadZone(inputValue);
        }

        [TestCase(Constants.AxisMaxValue, ExpectedResult = Constants.AxisMaxValue, TestName = "SensitivityHelper (Init): Max returns Max")]
        [TestCase(Constants.AxisMinValue, ExpectedResult = Constants.AxisMinValue, TestName = "SensitivityHelper (Init): Min returns Min")]
        [TestCase(0, ExpectedResult = 0, TestName = "SensitivityHelper (Init): 0 returns 0")]
        public long SensitivityHelperInitTests(long inputValue)
        {
            var helper = new SensitivityHelper();
            return helper.ApplyRangeSensitivity(inputValue);
        }

        [TestCase(Constants.AxisMaxValue, 50, ExpectedResult = Constants.AxisMaxValue, TestName = "SensitivityHelper (50): Max returns Max")]
        [TestCase(Constants.AxisMinValue, 50, ExpectedResult = Constants.AxisMinValue, TestName = "SensitivityHelper (50): Min returns Min")]
        [TestCase(Constants.AxisMaxValue, 20, ExpectedResult = Constants.AxisMaxValue, TestName = "SensitivityHelper (20): Max returns Max")]
        [TestCase(Constants.AxisMinValue, 20, ExpectedResult = Constants.AxisMinValue, TestName = "SensitivityHelper (20): Min returns Min")]
        public long SensitivityHelperValueTests(long inputValue, int percentage)
        {
            var helper = new SensitivityHelper {Percentage = percentage};
            return helper.ApplyRangeSensitivity(inputValue);
        }

        [TestCase(Constants.AxisMaxValue, 0, ExpectedResult = new long[]{Constants.AxisMaxValue, 0}, TestName = "CircularDeadZoneHelper (Init): Max returns Max")]
        [TestCase(Constants.AxisMinValue, 0, ExpectedResult = new long[]{Constants.AxisMinValue, 0}, TestName = "CircularDeadZoneHelper (Init): Min returns Min")]
        [TestCase(0, 0, ExpectedResult = new long[]{0, 0}, TestName = "CircularDeadZoneHelper (Init): 0 returns 0")]
        public long[] CircularDeadZoneInitTests(long x, long y)
        {
            var helper = new CircularDeadZoneHelper();
            return helper.ApplyRangeDeadZone(new[] {x, y});
        }

        [TestCase(Constants.AxisMaxValue, 0, 50, ExpectedResult = new long[] { Constants.AxisMaxValue, 0 }, 
            TestName = "CircularDeadZoneHelper (50): Max X returns Max")]

        [TestCase(Constants.AxisMinValue, 0, 50, ExpectedResult = new long[] { Constants.AxisMinValue, 0 },
            TestName = "CircularDeadZoneHelper (50): Min X returns Min")]

        [TestCase(0, Constants.AxisMaxValue, 50, ExpectedResult = new long[] { 0, Constants.AxisMaxValue }, 
            TestName = "CircularDeadZoneHelper (50): Max Y returns Max")]

        [TestCase(0, Constants.AxisMinValue, 50, ExpectedResult = new long[] { 0, Constants.AxisMinValue },
            TestName = "CircularDeadZoneHelper (50): Min Y returns Min")]

        [TestCase(0, 0, 50, ExpectedResult = new long[] { 0, 0 }, 
            TestName = "CircularDeadZoneHelper (50): 0 returns 0")]

        [TestCase(Constants.AxisMaxValue, 0, 20, ExpectedResult = new long[] { Constants.AxisMaxValue, 0 }, 
            TestName = "CircularDeadZoneHelper (20): Max X returns Max")]

        [TestCase(Constants.AxisMinValue, 0, 20, ExpectedResult = new long[] { Constants.AxisMinValue, 0 }, 
            TestName = "CircularDeadZoneHelper (20): Min X returns Min")]

        [TestCase(0, Constants.AxisMaxValue, 20, ExpectedResult = new long[] { 0, Constants.AxisMaxValue }, 
            TestName = "CircularDeadZoneHelper (20): Max Y returns Max")]

        [TestCase(0, Constants.AxisMinValue, 20, ExpectedResult = new long[] { 0, Constants.AxisMinValue }, 
            TestName = "CircularDeadZoneHelper (20): Min Y returns Min")]

        [TestCase(0, 0, 20, ExpectedResult = new long[] { 0, 0 },
            TestName = "CircularDeadZoneHelper (20): 0 returns 0")]

        [TestCase(16384, 0, 50, ExpectedResult = new long[] { 1, 0 }, 
            TestName = "CircularDeadZoneHelper (50): Positive X values above 16383 are outside DZ")]

        [TestCase(-16384, 0, 50, ExpectedResult = new long[] { -1, 0 },
            TestName = "CircularDeadZoneHelper (50): Negative X values below 16383 are outside DZ")]

        [TestCase(16383, 0, 50, ExpectedResult = new long[] { 0, 0 },
            TestName = "CircularDeadZoneHelper (50): Positive X values below 16384 are inside DZ")]

        [TestCase(-16383, 0, 50, ExpectedResult = new long[] { 0, 0 }, 
            TestName = "CircularDeadZoneHelper (50): Negative X values below 16384 are inside DZ")]

        [TestCase(0, 16384, 50, ExpectedResult = new long[] { 0, 1 }, 
            TestName = "CircularDeadZoneHelper (50): Positive Y values above 16383 are outside DZ")]

        [TestCase(0, -16384, 50, ExpectedResult = new long[] { 0 , -1 },
            TestName = "CircularDeadZoneHelper (50): Negative Y values below 16383 are outside DZ")]

        [TestCase(0, 16383, 50, ExpectedResult = new long[] { 0, 0 },
            TestName = "CircularDeadZoneHelper (50): Positive Y values below 16384 are inside DZ")]

        [TestCase(0, -16383, 50, ExpectedResult = new long[] { 0, 0 }, 
            TestName = "CircularDeadZoneHelper (50): Negative Y values below 16384 are inside DZ")]

        [TestCase(-16383, -16383, 50, ExpectedResult = new long[] { 0, 0 }, 
            TestName = "CircularDeadZoneHelper (50): Negative Y values below 16384 are inside DZ")]

        public long[] CircularDeadZoneValueTests(long x, long y, int percentage)
        {
            var helper = new CircularDeadZoneHelper {Percentage = percentage};
            return helper.ApplyRangeDeadZone(new[] {x, y});
        }

        // These test check that the deadzone is somewhat circular...
        // If passing +/- 16383 was INSIDE the DZ when passed for one axis...
        // (As tested by other tests)
        // ... but OUTSIDE the DZ when passed for both axes...
        // ... then the DZ cannot be square.
        [TestCase(-16383, -16383, 50,
            TestName = "CircularDeadZoneHelper (50): Deadzone appears Circular (Negative)")]
        [TestCase(16383, 16383, 50,
            TestName = "CircularDeadZoneHelper (50): Deadzone appears Circular (Positive)")]
        public void CircularDzOutsideTests(long x, long y, int percentage)
        {
            var helper = new CircularDeadZoneHelper { Percentage = percentage };
            var result = helper.ApplyRangeDeadZone(new long[] { -16383, -16383 });
            Assert.AreNotEqual(new long[] { 0, 0 }, result);
        }
    }
}

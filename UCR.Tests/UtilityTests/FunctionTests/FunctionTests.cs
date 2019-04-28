using HidWizards.UCR.Core.Utilities;
using NUnit.Framework;

namespace HidWizards.UCR.Tests.UtilityTests.FunctionTests
{
    [TestFixture]
    public class FunctionTests
    {
        [TestCase(0, ExpectedResult = 0, TestName = "Invert: 0 returns 0")]
        [TestCase(Constants.AxisMaxValue, ExpectedResult = Constants.AxisMinValue, TestName = "Invert: Max returns Min")]
        [TestCase(Constants.AxisMinValue, ExpectedResult = Constants.AxisMaxValue, TestName = "Invert: Min returns Max")]
        [TestCase(1, ExpectedResult = -1, TestName = "Invert: 1 returns -1")]
        [TestCase(-1, ExpectedResult = 1, TestName = "Invert: -1 returns 1")]
        public short InvertTests(short inputValue)
        {
            return Functions.Invert(inputValue);
        }

        [TestCase(Constants.AxisMinValue - 1, ExpectedResult = Constants.AxisMinValue, TestName = "ClampAxisRange: Greater than Max returns Max")]
        [TestCase(Constants.AxisMaxValue + 1, ExpectedResult = Constants.AxisMaxValue, TestName = "ClampAxisRange: Less than Min returns Min")]
        [TestCase(Constants.AxisMinValue, ExpectedResult = Constants.AxisMinValue, TestName = "ClampAxisRange: Min returns Min")]
        [TestCase(Constants.AxisMaxValue, ExpectedResult = Constants.AxisMaxValue, TestName = "ClampAxisRange: Max returns Max")]
        [TestCase(0, ExpectedResult = 0, TestName = "ClampAxisRange: 0 returns 0")]
        [TestCase(1, ExpectedResult = 1, TestName = "ClampAxisRange: 1 returns 1")]
        [TestCase(-1, ExpectedResult = -1, TestName = "ClampAxisRange: -1 returns -1")]
        public short ClampTests(int inputValue)
        {
            return Functions.ClampAxisRange(inputValue);
        }

        [TestCase(Constants.AxisMaxValue, true, ExpectedResult = Constants.AxisMaxValue, TestName = "SplitAxis (High): Max returns Max")]
        [TestCase(0, true, ExpectedResult = Constants.AxisMinValue, TestName = "SplitAxis (High): 0 returns Min")]
        [TestCase(Constants.AxisMinValue, false, ExpectedResult = Constants.AxisMaxValue, TestName = "SplitAxis (Low): Min returns Max")]
        [TestCase(0, true, ExpectedResult = Constants.AxisMinValue, TestName = "SplitAxis (Low): 0 returns Min")]
        [TestCase(Constants.AxisMinValue, true, ExpectedResult = Constants.AxisMinValue, TestName = "SplitAxis (High): Negative values return Min")]
        [TestCase(Constants.AxisMaxValue, false, ExpectedResult = Constants.AxisMinValue, TestName = "SplitAxis (Low): Positive values return Min")]
        [TestCase(1, true, ExpectedResult = -32766, TestName = "SplitAxis (High): 1 returns -32766")]
        [TestCase(-1, false, ExpectedResult = -32766, TestName = "SplitAxis (Low): -1 returns -32766")]
        public long SplitTests(short inputValue, bool positiveRange)
        {
            return Functions.SplitAxis(inputValue, positiveRange);
        }
    }
}

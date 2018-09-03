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
        [TestCase(0, ExpectedResult = 0, TestName = "Invert: 0 returns 0")]
        [TestCase(Constants.AxisMaxValue, ExpectedResult = Constants.AxisMinValue, TestName = "Invert: Max returns Min")]
        [TestCase(Constants.AxisMinValue, ExpectedResult = Constants.AxisMaxValue, TestName = "Invert: Min returns Max")]
        [TestCase(1, ExpectedResult = -1, TestName = "Invert: 1 returns -1")]
        [TestCase(-1, ExpectedResult = 1, TestName = "Invert: -1 returns 1")]
        public long InvertTests(long inputValue)
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
        public long ClampTests(long inputValue)
        {
            return Functions.ClampAxisRange(inputValue);
        }

        [TestCase(Constants.AxisMaxValue, true, ExpectedResult = Constants.AxisMaxValue, TestName = "SplitAxis (High): Max returns Max")]
        [TestCase(0, true, ExpectedResult = Constants.AxisMinValue, TestName = "SplitAxis (High): 0 returns Min")]
        [TestCase(Constants.AxisMinValue, false, ExpectedResult = Constants.AxisMaxValue, TestName = "SplitAxis (Low): Min returns Max")]
        [TestCase(0, true, ExpectedResult = Constants.AxisMinValue, TestName = "SplitAxis (Low): 0 returns Min")]
        public long SplitTests(long value, bool positiveRange)
        {
            return Functions.SplitAxis(value, positiveRange);
        }
    }
}

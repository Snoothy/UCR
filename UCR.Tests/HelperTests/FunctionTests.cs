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
        [TestCase(0, ExpectedResult = 0, TestName = "Invert 0 Should be 0")]
        [TestCase(Constants.AxisMaxValue, ExpectedResult = Constants.AxisMinValue, TestName = "Invert Max should be Min")]
        [TestCase(Constants.AxisMinValue, ExpectedResult = Constants.AxisMaxValue, TestName = "Invert of Min should be Max")]
        [TestCase(1, ExpectedResult = -1, TestName = "Invert of 1 should be -1")]
        [TestCase(-1, ExpectedResult = 1, TestName = "Invert of -1 should be 1")]
        public long InvertTests(long inputValue)
        {
            return Functions.Invert(inputValue);
        }

        [TestCase(Constants.AxisMinValue - 1, ExpectedResult = Constants.AxisMinValue, TestName = "Greater than Max Clamp should return Max")]
        [TestCase(Constants.AxisMaxValue + 1, ExpectedResult = Constants.AxisMaxValue, TestName = "Less than Min Clamp should return Min")]
        [TestCase(Constants.AxisMinValue, ExpectedResult = Constants.AxisMinValue, TestName = "Min Clamp should return Min")]
        [TestCase(Constants.AxisMaxValue, ExpectedResult = Constants.AxisMaxValue, TestName = "Max Clamp should return Max")]
        [TestCase(0, ExpectedResult = 0, TestName = "Clamping 0 should return 0")]
        [TestCase(1, ExpectedResult = 1, TestName = "Clamping 1 should return 1")]
        [TestCase(-1, ExpectedResult = -1, TestName = "Clamping -1 should return -1")]
        public long ClampTests(long inputValue)
        {
            return Functions.ClampAxisRange(inputValue);
        }
    }
}

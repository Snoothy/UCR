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
        public void DeadZoneHelperTest()
        {
            var helper = new DeadZoneHelper();
            // On initialize, the helper should work for DZ 0 without setting Percent
            Assert.AreEqual(helper.ApplyRangeDeadZone(Constants.AxisMaxValue), Constants.AxisMaxValue);
        }
    }
}

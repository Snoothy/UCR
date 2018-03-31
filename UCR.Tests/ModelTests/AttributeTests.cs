using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Plugins.ButtonToButton;
using NUnit.Framework;

namespace HidWizards.UCR.Tests.ModelTests
{
    [TestFixture]
    class AttributeTests
    {
        [Test]
        public void ButtonToButtonIoTest()
        {
            var plugin = new ButtonToButton();
            var inputs = plugin.InputCategories;
            var outputs = plugin.OutputCategories;

            Assert.AreEqual(inputs.Count, 1);
            Assert.AreEqual(inputs[0].Category, DeviceBindingCategory.Momentary);
            Assert.AreEqual(inputs[0].Name, "Button");
            Assert.AreEqual(outputs.Count, 1);
            Assert.AreEqual(outputs[0].Category, DeviceBindingCategory.Momentary);
            Assert.AreEqual(outputs[0].Name, "Button");
        }
    }
}

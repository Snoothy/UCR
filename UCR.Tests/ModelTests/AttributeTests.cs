using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Plugins.Remapper;
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

            Assert.That(inputs.Count, Is.EqualTo(1));
            Assert.That(inputs[0].Category, Is.EqualTo(DeviceBindingCategory.Momentary));
            Assert.That(inputs[0].Name, Is.EqualTo("Button"));
            Assert.That(outputs.Count, Is.EqualTo(1));
            Assert.That(outputs[0].Category, Is.EqualTo(DeviceBindingCategory.Momentary));
            Assert.That(outputs[0].Name, Is.EqualTo("Button"));
        }

        [Test]
        public void ButtonToButtonGuiMatrixTest()
        {
            var plugin = new ButtonToButton();

            var guiMatrix = plugin.GetGuiMatrix();
            var invertProperty = guiMatrix[0].PluginProperties[0];

            Assert.AreEqual(invertProperty.Name, "Invert");
        }
    }
}

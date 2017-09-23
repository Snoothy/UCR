using System;
using System.ComponentModel.Composition;
using System.Xml.Serialization;
using UCR.Core.Models.Binding;
using UCR.Core.Models.Plugin;

namespace UCR.Plugins.AxisToAxis
{
    [Export(typeof(Plugin))]
    public class AxisToAxis : Plugin
    {
        [XmlIgnore]
        public DeviceBinding InputAxis { get; set; }
        [XmlIgnore]
        public DeviceBinding OutputAxis { get; set; }

        public AxisToAxis()
        {
            InputAxis = InitializeInputMapping(InputChanged);
            OutputAxis = InitializeOutputMapping();
        }

        public override string PluginName()
        {
            return "Axis to axis";
        }

        private void InputChanged(long value)
        {
            WriteOutput(OutputAxis, value);
        }
    }
}

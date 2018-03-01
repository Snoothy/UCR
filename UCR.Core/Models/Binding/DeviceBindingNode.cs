using System.Collections.Generic;

namespace HidWizards.UCR.Core.Models.Binding
{
    public class DeviceBindingNode
    {
        public string Title { get; set; }
        public bool IsBinding { get; set; }
        public List<DeviceBindingNode> ChildrenNodes { get; set; }
        public DeviceBinding DeviceBinding { get; set; }
    }
}

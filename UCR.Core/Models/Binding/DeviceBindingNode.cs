using System.Collections.Generic;

namespace HidWizards.UCR.Core.Models.Binding
{
    public class DeviceBindingNode
    {
        public string Title { get; set; }
        public bool IsBinding => ChildrenNodes == null || ChildrenNodes.Count == 0;
        public List<DeviceBindingNode> ChildrenNodes { get; set; }
        public DeviceBindingInfo DeviceBindingInfo { get; set; }
    }
}
